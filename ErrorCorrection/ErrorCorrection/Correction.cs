using System;
using System.IO;
using System.Linq;

namespace ErrorCorrection;

public static class Correction
{
    private readonly static byte[,] matrix =
    {
        { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
        { 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
        { 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
        { 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0 },
        { 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0 },
        { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
    };


    public static void Encode(string inputFileName, string outputFile)
    {
        // otwórz plik wejściowy do odczytu
        using var inputFile = File.OpenRead(inputFileName);
        // otwórz plik wyjściowy do pisania
        using var fileStream = File.Open(outputFile, FileMode.Create);
        int message;

        // czytaj wiadomość po jednym bajcie
        while ((message = inputFile.ReadByte()) != -1)
        {
            // zrzutuj wiadomość na bajt oraz stwórz zmienne pomocnicze
            var messageByte = (byte)message;
            var binaryMessage = new byte[8];
            var check = new byte[8];

            // zamień liczbe na postać binarną(ośmio elementowa tablica zero i jedynek)
            for (var i = 7; i >= 0; i--)
            {
                binaryMessage[i] = (byte)(messageByte % 2);
                messageByte /= 2;
            }

            // wykonaj mnożenie przez macierz(jako mnożenie użyto operacji AND(&), a dodawanie modulo 2 - XOR(^)) 
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    check[i] ^= (byte)(binaryMessage[j] & matrix[i, j]);
                }
            }

            // zapisz odczytany bajt wiadomość do pliku wyjściowego
            fileStream.WriteByte((byte)message);

            // zamień bity parzystości z postaci binarnej do postaci liczbowej
            byte msg = 0;
            for (var j = 7; j >= 0; j--)
            {
                msg |= (byte)(check[7 - j] << j);
            }

            // zapisz bity parzystośći do pliku zaraz po bajcie wiadomości
            fileStream.WriteByte(msg);
        }
    }

    public static void Decode(string inputFile, string outputFile)
    {
        // otwórz plik wejściowy do odczytu
        using var encodedFile = File.OpenRead(inputFile);
        // otwórz plik wyjściowy do pisania
        using var decodedFile = File.Open(outputFile, FileMode.Create);
        // stwórz zmienne pomocniczne
        var buffer = new byte[2];
        var result = new byte[8];

        // odczytaj 2 bajty zakodowanej wiadomości(bajt wiadomści i bajt zawierający bity parzystości)
        while (encodedFile.Read(buffer) != 0)
        {
            // zamień dwa bajty na jedną 16-bitową liczbę
            var messageShort = BitConverter.ToUInt16(buffer.Reverse().ToArray());
            var message = new byte[16];

            // zamień 16-bitową liczbę na tablice bajtów zawierającą wartości binarne
            for (var i = 15; i >= 0; i--)
            {
                message[i] = (byte)(messageShort % 2);
                messageShort /= 2;
            }

            // obliczamy macierz błędów (pokazuje gdzie są błędy)
            for (var i = 0; i < 8; i++)
            {
                result[i] = 0;
                for (var j = 0; j < 16; j++)
                {
                    result[i] ^= (byte)(message[j] & matrix[i, j]);
                }
            }

            // zamień tablicę bitów na jedną liczbę jednobajtową 
            // obliczona wartość różna od zera wskazuje na kolumnę
            // dlatego kolumna w macierzy nie może zawierać samych zer, było by to myle z sytuacją braku błędu
            var resultValue = 0;
            for (var j = 7; j >= 0; j--)
            {
                resultValue |= (byte)(result[7 - j] << j);
            }

            // wykryto błąd
            if (resultValue != 0)
            {
                var oneError = false;

                // sprawdzanie czy jest jeden błąd i korekcja pojedynczego bitu
                for (var i = 0; i < 16; i++)
                {
                    // oblczanie wartości kolumny jako liczby(liczba binarana zapisana w kolumnie)               
                    byte columnValue = 0;
                    for (var j = 7; j >= 0; j--)
                    {
                        columnValue |= (byte)(matrix[7 - j, i] << j);
                    }

                    // jeżeli obliczona wartość błędu jest równa obliczonej wartości kolumny znaleźliśmy pojedyńczy bład i możemy go poprawić
                    if (columnValue == resultValue)
                    {
                        // odwaracamy znaleziony bit błedu(0 XOR 1 = 1; 1 XOR 1 = 0)
                        buffer[0] ^= (byte)(1 << (7 - i));
                        // zmienna pomocnicza mówiąca, że znaleziono jeden bład
                        oneError = true;
                    }
                }

                var toBreak = false;
                // sprawdzanie czy są dwa błedy i korekcja dwóch bitów
                if (!oneError)
                {
                    for (var i = 0; i < 16; i++)
                    {
                        for (var j = 0; j < 16; j++)
                        {

                            // obliczanie wartości 2 kolumn (tak samo jak w przypadku pojedyńczej kolumny w sytuacji jednego błedu)
                            var firstColumnValue = 0;
                            var secondColumnValue = 0;

                            for (var k = 7; k >= 0; k--)
                            {
                                firstColumnValue |= (byte)(matrix[7 - k, i] << k);
                                secondColumnValue |= (byte)(matrix[7 - k, j] << k);
                            }

                            // jeżeli suma wartości 2 kolumn jest równa wartośći błedu znaleźliśmy błędy i możemy je naprawić
                            // ważne jest aby żadne 2 kolumny macierzy nie dawały takiej samej sumy
                            // dodawanie wykonujemy modulo 2 (XOR)
                            if ((byte)(firstColumnValue ^ secondColumnValue) == resultValue)
                            {
                                // korygujemy 2 błedy (tak samo jak w sytuacji jednego błedu)
                                buffer[0] ^= (byte)(1 << (7 - i));
                                buffer[0] ^= (byte)(1 << (7 - j));
                                // ustawiamy zjemmną sygnalizująca wyjście z pętli
                                toBreak = true;
                                break;
                            }
                        }

                        if (toBreak)
                        {
                            break;
                        }
                    }
                }

                // jeżeli dojdziemy tutaj bez naprawienia błędu oznacza to, że błędów było więcej niż 2
            }

            // zapisz bajt wiadomości do pliku wyjścia
            decodedFile.WriteByte(buffer[0]);
        }
    }
}
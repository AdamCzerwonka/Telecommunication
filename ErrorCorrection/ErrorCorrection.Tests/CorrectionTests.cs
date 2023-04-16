using ErrorCorrection.Lib;
using Xunit.Abstractions;

namespace ErrorCorrection.Tests;

public class CorrectionTests
{
   private readonly ITestOutputHelper _testOutputHelper;

   public CorrectionTests(ITestOutputHelper testOutputHelper)
   {
      _testOutputHelper = testOutputHelper;
   }

   [Fact]
   public void TestDecodedResult_ShouldBeIdenticalAsInput_WhenNoModificationsApplied()
   {
      var input = "konstantynopolitańczykowianeczka"u8.ToArray();
      const string clearInputFileName = "randomName.txt";
      const string encodedFileName = "randomOutput.txt";
      const string decodedFileName = "randomOutput2.txt";
      File.WriteAllBytes(clearInputFileName, input);
      
      Correction.Encode(clearInputFileName, encodedFileName);
      
      Correction.Decode(encodedFileName, decodedFileName);

      var result = File.ReadAllBytes(decodedFileName);
      Assert.Equal(input, result);
   } 
   
   [Fact]
   public void TestDecodedResult_ShouldBeIdenticalAsInput_WhenLastByteHasTwoBitsChanged()
   {
      var input = "konstantynopolitańczykowianeczka"u8.ToArray();
      const string clearInputFileName = "randomName.txt";
      const string encodedFileName = "randomOutput.txt";
      const string decodedFileName = "randomOutput2.txt";
      File.WriteAllBytes(clearInputFileName, input);
      
      Correction.Encode(clearInputFileName, encodedFileName);

      var encodedBytes = File.ReadAllBytes(encodedFileName);
      encodedBytes[^1] = 0x8b;
      File.WriteAllBytes(encodedFileName, encodedBytes);
      
      
      Correction.Decode(encodedFileName, decodedFileName);

      var result = File.ReadAllBytes(decodedFileName);
      Assert.Equal(input, result);
   } 
   
   [Fact]
   public void TestDecodedResult_ShouldBeIdenticalAsInput_WhenLastByteHasOneBitChanged()
   {
      var input = "konstantynopolitańczykowianeczka"u8.ToArray();
      const string clearInputFileName = "randomName.txt";
      const string encodedFileName = "randomOutput.txt";
      const string decodedFileName = "randomOutput2.txt";
      File.WriteAllBytes(clearInputFileName, input);
      
      Correction.Encode(clearInputFileName, encodedFileName);

      var encodedBytes = File.ReadAllBytes(encodedFileName);
      encodedBytes[^1] = 0x9f;
      File.WriteAllBytes(encodedFileName, encodedBytes);
      
      
      Correction.Decode(encodedFileName, decodedFileName);

      var result = File.ReadAllBytes(decodedFileName);
      Assert.Equal(input, result);
   } 
}
namespace WA90.Tests
{
    public class Hello_Tests
    {
        [Fact]
        public void SayHello_Test()
        {
            // Arrange
            bool varBool;

            // Act
            varBool = true;

            // Assert
            Assert.True(varBool, "No es verdadero");
        }
    }
}
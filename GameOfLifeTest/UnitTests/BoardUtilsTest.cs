using Xunit;
using System.Collections.Generic;

public class BoardUtilsTests
{
    [Fact]
    public void ConvertTo2DArray_ShouldConvertNestedListTo2DArray()
    {
        // Arrange
        var state = new List<List<bool>>
        {
            new List<bool> { true, false },
            new List<bool> { false, true }
        };

        // Act
        var result = BoardUtils.ConvertTo2DArray(state);

        // Assert
        Assert.True(result[0, 0]);
        Assert.False(result[0, 1]);
    }

    [Fact]
    public void SerializeState_ShouldReturnSerializedString()
    {
        // Arrange
        var state = new List<List<bool>>
        {
            new List<bool> { true, false },
            new List<bool> { false, true }
        };

        // Act
        var result = BoardUtils.SerializeState(state);

        // Assert
        Assert.Equal("10,01", result);
    }
}

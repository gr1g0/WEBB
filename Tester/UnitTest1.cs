namespace Prog.Services.Tests

{
    using Xunit;
    public class SortTests
    {
        [Fact]
        public void Sort_ArrayWithMultipleElements_SortsCorrectly()
        {
            int[] array = { 12, 11, 13, 5, 6, 7 };
            LinkedList<int> list = new LinkedList<int>();
            for (int i = 0; i < array.Length; i++){
                list.Add(array[i]);
            }
            list.Sort();
            Assert.Equal(new List<int> { 5, 6, 7, 11, 12, 13 }, list);
        }

        [Fact]
        public void Sort_EmptyArray_ReturnsEmptyList()
        {
            LinkedList<int> list = new LinkedList<int>();

            list.Sort();

            Assert.Empty(list);
        }

        [Fact]
        public void Sort_SingleElementArray_ReturnsSingleElementList()
        {
            LinkedList<int> list = new LinkedList<int>();
            list.Add(1);
            list.Sort();
            Assert.Equal(new List<int> { 1 }, list);
        }

        [Fact]
        public void Sort_ArrayWithNegativeElements_SortsCorrectly()
        {
            // Arrange
            int[] array = { -3, -1, -2, -4, -5 };
            LinkedList<int> list = new LinkedList<int>();
            for (int i = 0; i < array.Length; i++){
                list.Add(array[i]);
            }
            list.Sort();
            Assert.Equal(new List<int> { -5, -4, -3, -2, -1 }, list);
        }
    }
}
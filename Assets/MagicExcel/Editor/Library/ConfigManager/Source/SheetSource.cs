//https://github.com/RickJiangShu
namespace MagicExcel {
    public class SheetSource {
        public string originalName;
        public string className;
        public string[,] matrix;
        public int Row => matrix.GetLength(0);
        public int Column => matrix.GetLength(1);
    }
}
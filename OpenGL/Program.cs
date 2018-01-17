namespace OpenGL
{
    public static class Program
    {
        //private static readonly IProgram program = new RotatingCubes();
        //private static readonly IProgram program = new HeightMapProgram();
        private static readonly IProgram program = new TransparenceProgram();

        private static void Main()
        {
            program.Run();
        }
    }
}
namespace Lunner
{
    //ゲーム内で扱う入力キー
    public enum InputCommand
    {
        None = 0,
        Left,
        Right,
        Up,
        Down,
        Max,
    }

    //ランシーンの状態
    public enum RunGameState
    {
        None,
        Start,
        Runnning,
        Goal,
    }

    //レール上の位置
    public enum RailPositionIndex
    {
        Left,
        Center,
        Right,
        Up,
        Down,
    }

    public class Defines
    {
        public const int DefaultFps = 60;
    }
}
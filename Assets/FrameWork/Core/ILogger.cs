
namespace XFrameWork
{ 
public interface ILogger
{
        bool enable
        {
            get;
            set;
        }

        void Error(string info);

        void Print(string info);

        void Warning(string info);
}
}

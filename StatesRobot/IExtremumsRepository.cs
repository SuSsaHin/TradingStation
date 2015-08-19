using System.Collections.Generic;
using TradeTools;

namespace StatesRobot
{
    public interface IExtremumsRepository
    {
        ICollection<Extremum> FirstMinimums { get; }
        ICollection<Extremum> FirstMaximums { get; }
        ICollection<Extremum> SecondMinimums { get; }
        ICollection<Extremum> SecondMaximums { get; }
    }
}
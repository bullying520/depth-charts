using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    // player
    public interface IPlayer
    {
        int Number { get; }
        string FirstName { get; }
        string LastName { get; }
        string FullName { get; }
    }

    public class Player : IPlayer
    {
        public Player(string firstName, string lastName, int number)
        {
            FirstName = firstName;
            LastName = lastName;
            Number = number;
        }

        public int Number { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => $"{FirstName} {LastName}";

        public static bool operator ==(Player a, Player b) => a.Equals(b);
        public static bool operator !=(Player a, Player b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (!(obj is IPlayer player))
                return false;

            return Number == player.Number;
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public override string ToString() => $"#{Number}, {FullName}";
    }

    // dummy player is a place holder that when you insert a live player into an empty list at position X, the dummy player will be inserted in the front of the live player
    //public class DummyPlayer : Player
    //{
    //    public DummyPlayer() : base("", "", 0) { }

    //    public override bool Equals(object obj)
    //    {
    //        return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }
    //}

    // position rank list
    public interface IPositionRankList
    {
        string Position { get; }
        int Count { get; }

        void AddPlayerToPositionList(IPlayer player, int? positionDepth = null);
        IPlayer RemovePlayerFromPositionList(IPlayer player);
        IEnumerable<IPlayer> GetBackups(IPlayer player);
        IEnumerable<IPlayer> GetFullList();
    }

    public class PositionRankList : IPositionRankList
    {
        private readonly List<IPlayer> list;

        public PositionRankList(string position)
        {
            Position = position;
            list = new List<IPlayer>();
        }

        public string Position { get; }

        public int Count => list.Count;

        public void AddPlayerToPositionList(IPlayer player, int? positionDepth = null)
        {
            if (list.Contains(player))
            {
                throw new ArgumentException($"The player {player.FullName} already exists");
            }

            var newPositionDepth = positionDepth ?? list.Count;
            if (newPositionDepth >= list.Count)
            {
                list.Add(player);
                return;
            }

            list.Insert(newPositionDepth, player);
        }

        public IEnumerable<IPlayer>GetBackups(IPlayer player)
        {
            if (!list.Contains(player)) { return null; }

            // index must be greater than 0, as we already check the player existed first.
            var index = list.IndexOf(player);
            return list.GetRange(index + 1, list.Count - 1 - index);            
        }

        public IEnumerable<IPlayer> GetFullList()
        {
            return list;
        }

        public IPlayer RemovePlayerFromPositionList(IPlayer player)
        {
            if (!list.Contains(player)) { return null; }

            list.Remove(player);
            return player;
        }
    }

    // sport
    public interface ISport
    {
        string Name { get; }
        IEnumerable<string> Positions { get; }
    }

    public class NFL : ISport
    {
        // ref: https://www.rookieroad.com/football/positions/
        public enum Position
        {
            QB,
            RB,
            FB,
            WR,
            TE,
            LT,
            RT,
            LG,
            RG,
            C
        }
        public string Name => "NFL";

        public IEnumerable<string> Positions => Enum.GetNames(typeof(Position));
    }

    public class Soccer : ISport
    {
        public enum Position
        {
            ST,
            CF,
            LW,
            RW,
            CAM,
            CDM,
            CM,
            LB,
            CB,
            RB,
            GK
        }
        public string Name => "Soccer";

        public IEnumerable<string> Positions => Enum.GetNames(typeof(Position));
    }

    // IDepthCharts interface, deine the common methods for the depth charts
    public interface IDepthCharts<T> where T: IPositionRankList
    {
        void AddPlayerToDepthChart(string position, IPlayer player, int? positionDepth = null);
        IPlayer RemovePlayerFromDepthChart(string position, IPlayer player);
        IEnumerable<IPlayer> GetBackups(string position, IPlayer player);

        // add this method to retrieve all the players in the same position
        IEnumerable<IPlayer> GetFullPositionChart(string position);

        // key for the position and enumerable for the players
        IDictionary<string, IEnumerable<IPlayer>> GetFullDepthCharts();

        string PrintFullDepthCharts(Func<IDictionary<string, IEnumerable<IPlayer>>, string> printFunc);
    }

    // depth charts, any class implements IPositionRankList is accpeted
    public class DepthCharts<T> : IDepthCharts<T> where T : IPositionRankList
    {
        // use dictionary to reteive the ranlist based on the position, and manuplate it with player.
        private readonly Dictionary<string, T> rankListDictionary = new Dictionary<string, T>();
        private readonly ISport _sport = null;

        public DepthCharts(ISport sport, IPositionRankListFactory<T> factory)
        {
            _sport = sport;

            // since most of the positions in the sport are developed and fixed, we don't need to add new position dynamically. Otherwise, we may need to design an interface method to add / remove position
            foreach (var position in sport.Positions.Distinct())
            {
                rankListDictionary[position] = factory.CreateInstance(position);
            }
        }

        public void AddPlayerToDepthChart(string position, IPlayer player, int? positionDepth = null)
        {
            ValidatePosition(position);
            rankListDictionary[position].AddPlayerToPositionList(player, positionDepth);
        }

        public IPlayer RemovePlayerFromDepthChart(string position, IPlayer player)
        {
            ValidatePosition(position);
            return rankListDictionary[position].RemovePlayerFromPositionList(player);
        }

        public IEnumerable<IPlayer> GetBackups(string position, IPlayer player)
        {
            ValidatePosition(position);
            return rankListDictionary[position].GetBackups(player);
        }

        public IDictionary<string, IEnumerable<IPlayer>> GetFullDepthCharts()
        {
            return rankListDictionary.ToDictionary(item => item.Key, item => item.Value.GetFullList());            
        }

        public void ValidatePosition(string position)
        {
            if (!rankListDictionary.Keys.Contains(position))
            {
                throw new ArgumentOutOfRangeException($"The position {position} doesn't exist in the {_sport.Name}");
            }
        }

        public IEnumerable<IPlayer> GetFullPositionChart(string position)
        {
            ValidatePosition(position);
            return rankListDictionary[position].GetFullList();
        }

        public string PrintFullDepthCharts(Func<IDictionary<string, IEnumerable<IPlayer>>, string> printFunc)
        {
            return printFunc(GetFullDepthCharts());
        }
    }

    // PositionRankListFactory
    public interface IPositionRankListFactory<T> where T: IPositionRankList
    {
        T CreateInstance(string position);
    }

    // we have to define how the IPositionRankList instance is created. The position has to be passed in as the constructor parameter
    public class PositionRankListFactory<T> : IPositionRankListFactory<T> where T : IPositionRankList
    {
        public T CreateInstance(string position)
        {
            return (T)Activator.CreateInstance(typeof(T), position);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private IPlayer TomBrady;
        private IPlayer TomBrady2;
        private IPlayer TomBrady3;
        private IPositionRankList positionList;
        private PositionRankListFactory<PositionRankList> factory;
        private DepthCharts<PositionRankList> depthCharts;
        private NFL nfl;
        private Soccer soccer;
        private IEnumerable<string> NFLPositions;
        private IEnumerable<string> soccerPositions;

        [TestInitialize]
        public void SetUp()
        {
            TomBrady = new Player("Tom", "Brady", 12);
            TomBrady2 = new Player("Tom2", "Brady2", 22);
            TomBrady3 = new Player("Tom3", "Brady3", 32);
            positionList = new PositionRankList("ff");

            factory = new PositionRankListFactory<PositionRankList>();
            depthCharts = new DepthCharts<PositionRankList>(new NFL(), "Offense", factory);

            nfl = new NFL();
            soccer = new Soccer();

            NFLPositions = nfl.Positions;
            soccerPositions = soccer.Positions;
        }

        [TestMethod]
        public void PlayerShouldShowFullNameWithSpace()
        {
            Assert.AreEqual(TomBrady.FullName, "Tom Brady");
        }

        public void TestPlayerToString()
        {
            Assert.AreEqual(TomBrady.ToString(), "#12, Tom Brady");
        }

        [TestMethod]
        public void PlayerShouldEqualWhenTheirNumbersEqual()
        {
            var TomBrady2 = new Player("Tom2", "Brady2", 12);
            Assert.AreEqual(TomBrady, TomBrady2);
        }

        [TestMethod]
        public void PlayerShouldNotBeEqualedWhenTheirNumbersNotEqual()
        {
            Assert.AreNotEqual(TomBrady, TomBrady2);
        }

        [TestMethod]
        public void PlayerShouldEqualWithOperatorWhenTheirNumbersEqual()
        {
            var TomBrady = new Player("Tom", "Brady", 12);
            var TomBrady2 = new Player("Tom2", "Brady2", 12);
            Assert.IsTrue(TomBrady == TomBrady2);
        }

        [TestMethod]
        public void PlayerShouldNotBeEqualedWithOperatorWhenTheirNumbersNotEqual()
        {
            var TomBrady = new Player("Tom", "Brady", 12);
            var TomBrady2 = new Player("Tom", "Brady", 13);
            Assert.IsTrue(TomBrady != TomBrady2);
        }

        // Position List Unit tests
        [TestMethod]
        public void TestAddFirstPlayerToList()
        {
            positionList.AddPlayerToPositionList(TomBrady);

            Assert.AreEqual(positionList.Count, 1);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
        }

        [TestMethod]
        public void TestAddTwoPlayersToList()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);

            Assert.AreEqual(positionList.Count, 2);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady2.Number);
        }

        [TestMethod]
        public void TestInsertPlayerAtFirst()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2, 0);

            Assert.AreEqual(positionList.Count, 2);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady2.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady.Number);
        }

        [TestMethod]
        public void TestInsertPlayerAtLast()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3, 2);

            Assert.AreEqual(positionList.Count, 3);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestInsertPlayerInTheMiddle()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3, 1);

            Assert.AreEqual(positionList.Count, 3);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady2.Number);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestAddDuplciatePlayerToList()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady);
        }

        [TestMethod]
        public void TestInsertPlayerAtLargePosiionIndex()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2, 999);

            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady2.Number);
        }

        [TestMethod]
        public void TestRemoveNotExistingPlayer()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            Assert.IsNull(positionList.RemovePlayerFromPositionList(TomBrady2));
        }

        [TestMethod]
        public void TestRemoveTheOnlyExistingPlayer()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.RemovePlayerFromPositionList(TomBrady);

            Assert.AreEqual(positionList.Count, 0);
        }

        [TestMethod]
        public void TestRemoveTheFirstFromThreePlayers()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            positionList.RemovePlayerFromPositionList(TomBrady);

            Assert.AreEqual(positionList.Count, 2);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady2.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestRemoveMiddleFromThreePlayers()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            positionList.RemovePlayerFromPositionList(TomBrady2);

            Assert.AreEqual(positionList.Count, 2);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestRemoveLastFromThreePlayers()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            var player = positionList.RemovePlayerFromPositionList(TomBrady3);

            Assert.AreEqual(positionList.Count, 2);
            Assert.AreEqual(positionList.GetFullList().First().Number, TomBrady.Number);
            Assert.AreEqual(positionList.GetFullList().Last().Number, TomBrady2.Number);

            Assert.AreEqual(player.Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestGetBacupsForNonExistingPlayer()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);

            var backups = positionList.GetBackups(TomBrady3);

            Assert.IsNull(backups);
        }

        [TestMethod]
        public void TestGetBackupsAfterTheFirst()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            var backups = positionList.GetBackups(TomBrady);

            Assert.AreEqual(backups.First().Number, TomBrady2.Number);
            Assert.AreEqual(backups.Last().Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestGetBackupsAfterTheSecond()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            var backups = positionList.GetBackups(TomBrady2);

            Assert.AreEqual(backups.Count(), 1);
            Assert.AreEqual(backups.First().Number, TomBrady3.Number);
        }

        [TestMethod]
        public void TestGetBackupsAfterTheLast()
        {
            positionList.AddPlayerToPositionList(TomBrady);
            positionList.AddPlayerToPositionList(TomBrady2);
            positionList.AddPlayerToPositionList(TomBrady3);

            var backups = positionList.GetBackups(TomBrady3);

            Assert.AreEqual(backups.Count(), 0);
        }

        [TestMethod]
        public void TestNFLPositions()
        {
            Assert.AreEqual(NFLPositions.Count(), 10);
            Assert.IsTrue(NFLPositions.Contains("QB"));
        }

        [TestMethod]
        public void TestSoccerPositions()
        {
            Assert.AreEqual(soccerPositions.Count(), 11);
            Assert.IsTrue(soccerPositions.Contains("GK"));
        }

        [TestMethod]
        public void TestPositionRankListFactory()
        {
            var rankList = new PositionRankListFactory<PositionRankList>().CreateInstance(NFL.Position.QB.ToString());
            Assert.IsTrue(rankList is IPositionRankList);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void TestValidatePositionWhenPositionNotExist()
        {
            depthCharts.ValidatePosition("none");
        }

        [TestMethod]
        public void AddPlayerToPositionShouldSucceed()
        {
            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);

            var qbList = depthCharts.GetFullPositionChart(qb);
            Assert.AreEqual(qbList.Count(), 1);
        }

        [TestMethod]
        public void AddSamePlayerToDifferentPositionShouldSucceed()
        {
            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);

            var fb = NFL.Position.FB.ToString();
            depthCharts.AddPlayerToDepthChart(fb, TomBrady);

            var qbList = depthCharts.GetFullPositionChart(qb);
            Assert.AreEqual(qbList.Count(), 1);

            var fbList = depthCharts.GetFullPositionChart(fb);
            Assert.AreEqual(fbList.Count(), 1);
        }

        [TestMethod]
        public void AddDifferentPlayerToDifferentPositionShouldSucceed()
        {
            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);

            var fb = NFL.Position.FB.ToString();
            depthCharts.AddPlayerToDepthChart(fb, TomBrady2);

            var qbList = depthCharts.GetFullPositionChart(qb);
            Assert.AreEqual(qbList.Count(), 1);
            Assert.AreEqual(qbList.First(), TomBrady);

            var fbList = depthCharts.GetFullPositionChart(fb);
            Assert.AreEqual(fbList.Count(), 1);
            Assert.AreEqual(fbList.First(), TomBrady2);
        }

        [TestMethod]
        public void TestPrintFullDepthCharts()
        {
            var sb = new StringBuilder();
            Func<IDictionary<string, IEnumerable<IPlayer>>, string> func = (dict) =>
            {
                foreach (var position in dict.Keys)
                {
                    var list = dict[position];
                    if (list.Count() == 0) { continue; }

                    var players = string.Join(", ", list.Select(player => $"({player})"));
                    sb.AppendLine($"{position} - {players}");
                }

                return sb.ToString();
            };

            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);
            depthCharts.AddPlayerToDepthChart(qb, TomBrady3);

            var fb = NFL.Position.FB.ToString();
            depthCharts.AddPlayerToDepthChart(fb, TomBrady2);

            var result = depthCharts.PrintFullDepthCharts(func);
            var expected = @"QB - (#12, Tom Brady), (#32, Tom3 Brady3)
FB - (#22, Tom2 Brady2)
";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestPrintBackups()
        {
            var sb = new StringBuilder();
            Func<IEnumerable<IPlayer>, string> func = (list) =>
            {
                var players = string.Join("\r\n", list.Select(player => $"{player}"));
                sb.AppendLine(players);

                return sb.ToString();
            };

            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);
            depthCharts.AddPlayerToDepthChart(qb, TomBrady2);
            depthCharts.AddPlayerToDepthChart(qb, TomBrady3);

            var result = depthCharts.PrintBackups(func, qb, TomBrady);
            var expected = @"#22, Tom2 Brady2
#32, Tom3 Brady3
";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestPrintBackupsNoList()
        {
            var sb = new StringBuilder();
            Func<IEnumerable<IPlayer>, string> func = (list) =>
            {
                if (list.Count() == 0)
                {
                    sb.AppendLine(DepthCharts<PositionRankList>.STR_NO_LIST);
                }
                else
                {
                    var players = string.Join("\r\n", list.Select(player => $"{player}"));
                    sb.AppendLine(players);
                }

                return sb.ToString();
            };

            var qb = NFL.Position.QB.ToString();
            depthCharts.AddPlayerToDepthChart(qb, TomBrady);
            depthCharts.AddPlayerToDepthChart(qb, TomBrady2);
            depthCharts.AddPlayerToDepthChart(qb, TomBrady3);

            var result = depthCharts.PrintBackups(func, qb, TomBrady3);
            var expected = @"<NO LIST>
";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestPrintFullDepthChartsNoList()
        {
            var sb = new StringBuilder();
            Func<IDictionary<string, IEnumerable<IPlayer>>, string> func = (dict) =>
            {
                foreach (var position in dict.Keys)
                {
                    var list = dict[position];
                    if (list.Count() == 0) { continue; }

                    var players = string.Join(", ", list.Select(player => $"({player})"));
                    sb.AppendLine($"{position} - {players}");
                }

                if (sb.Length == 0)
                {
                    return DepthCharts<PositionRankList>.STR_NO_LIST;
                }

                return sb.ToString();
            };

            var result = depthCharts.PrintFullDepthCharts(func);
            var expected = @"<NO LIST>";
            Assert.AreEqual(result, expected);
        }
    }
}

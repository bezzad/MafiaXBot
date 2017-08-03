using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaX.BotEngine
{
    public class Game
    {
        public long GroupId { get; set; }
        public Guid Id { get; set; }
        public int DayCount { get; protected set; }
        public GameStatus State { get; protected set; }
        public CultureInfo Culture { get; set; }
        public Dictionary<long, Player> Players { get; protected set; }
        public Dictionary<long, Player> PolicePlayers { get; protected set; }
        public Dictionary<long, Player> MafiaPlayers { get; protected set; }
        public Player GodfatherPlayer { get; protected set; }
        public Player DoctorPlayer { get; protected set; }
        public Player DetectivePlayer { get; protected set; }
        public Player GodPlayer { get; protected set; }
        public int MafiaPlayersCount { get; set; }
        public int PolicePlayersCount { get; set; }



        public Game(long chatId)
        {
            Id = Guid.NewGuid();
            GroupId = chatId;
            State = GameStatus.New;
            Culture = new CultureInfo("en");
            DayCount = 0;
            Players = new Dictionary<long, Player>();
            MafiaPlayers = new Dictionary<long, Player>();
            PolicePlayers = new Dictionary<long, Player>();
        }


        public void Start()
        {
            if (State == GameStatus.New)
            {
                State = GameStatus.Day;
                DayCount = 1;
            }
        }
        public void Abort()
        {
            State = GameStatus.Over;
        }
        public void Day()
        {
            if (State == GameStatus.Night)
            {
                DayCount++;
                State = GameStatus.Day;
            }
        }
        public void Night()
        {
            if (State == GameStatus.Day)
            {
                State = GameStatus.Night;
            }
        }
        public string RegisterPlayer(Player p)
        {
            if (State == GameStatus.New)
            {
                if (Players.ContainsKey(p.Id))
                {
                    return string.Format("The_0_already_exist_in_game".GetLocalized(p.User.Culture), p.User.Username);
                }

                Players.Add(p.Id, p);
                return string.Format("_0_added_to_game_successful.".GetLocalized(p.User.Culture), p.User.Username);
            }

            return "Game_already_started_before_your_registering".GetLocalized(p.User.Culture);
        }
        public string KillPlayer(int playerId)
        {
            if (Players.ContainsKey(playerId))
            {
                var player = Players[playerId];
                if (player.IsAlive)
                {
                    player.KillMe();
                    return string.Format("_0_killed".GetLocalized(Culture), player.User.Username);
                }
                return string.Format("_0_already_has_killed".GetLocalized(Culture), player.User.Username);
            }

            return "This_user_did_not_registered_before_starting_game".GetLocalized(Culture);
        }

        public string GetPlayersRoleCount()
        {
            if (Players.Count <= 5)
                return "The_count_of_players_must_be_more_than_5";

            MafiaPlayersCount = Players.Count / 3;
            PolicePlayersCount = Players.Count - MafiaPlayersCount;

            return "The_count_of_mafia_is_0_and_police_is_1".GetLocalized(Culture);
        }

        public async Task<string> InitPlayersRole()
        {
            if (Players.Count <= 5)
                return "The_count_of_players_must_be_more_than_5".GetLocalized(Culture);

            if (MafiaPlayersCount < 2)
                return "Mafia_count_is_less_than_2".GetLocalized(Culture);

            if (Players.Count - MafiaPlayersCount != PolicePlayersCount)
                return "The_sum_of_police_and_mafia_counts_is_not_equal".GetLocalized(Culture);

            Players = await Players.RandomCross();
            var playerKeys = Players.Keys.ToArray();
            var rand = new Random();

            //
            // select mafia
            for (int i = 0; i < MafiaPlayersCount; i++)
            {
                MafiaPlayers[playerKeys[i]] = Players[playerKeys[i]];
            }
            //
            // select police
            for (int i = MafiaPlayersCount; i < Players.Count; i++)
            {
                PolicePlayers[playerKeys[i]] = Players[playerKeys[i]];
            }
            //
            // select godfather
            var godfatherKey = playerKeys[rand.Next(0, MafiaPlayersCount - 1)];
            GodfatherPlayer = MafiaPlayers[godfatherKey];
            //
            // select detective and doctor
            long detectiveKey;
            long doctorKey;
            do
            {
                detectiveKey = playerKeys[rand.Next(MafiaPlayersCount, Players.Count - 1)];
                await Task.Delay(1);
                doctorKey = playerKeys[rand.Next(MafiaPlayersCount, Players.Count - 1)];
            } while (detectiveKey == doctorKey);

            GodfatherPlayer = MafiaPlayers[godfatherKey];

            return "".GetLocalized(Culture);
        }


    }
}

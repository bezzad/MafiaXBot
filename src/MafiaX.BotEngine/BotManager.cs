using System.Collections.Generic;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace MafiaX.BotEngine
{
    public class BotManager
    {
        #region Properties

        public TelegramBotClient Bot { get; set; }
        public Telegram.Bot.Types.User Me { get; set; }
        public Telegram.Bot.Types.Chat Group { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public string BotApiKey { get; set; }

        public IReplyMarkup VoteInlineKeyboard { get; set; }

        public IReplyMarkup CommonReplyKeyboard { get; set; }
        public IReplyMarkup DoctorReplyKeyboard { get; set; }
        public IReplyMarkup DetectiveReplyKeyboard { get; set; }
        public IReplyMarkup MafiaReplyKeyboard { get; set; }
        public IReplyMarkup GodfatherReplyKeyboard { get; set; }

        #endregion

        #region Constructors

        public BotManager()
        {
            CurrentCulture = new CultureInfo("en");
            InitKeyboards();
        }

        public BotManager(string apiKey) : this()
        {
            BotApiKey = apiKey;
        }

        #endregion

        #region Methods

        public async void StartListening()
        {
            Bot = new TelegramBotClient(BotApiKey);
            Bot.StartReceiving();
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.OnInlineQuery += Bot_OnInlineQuery;
            Bot.OnInlineResultChosen += Bot_OnInlineResultChosen;

            Me = await Bot.GetMeAsync();
        }

        private void Bot_OnInlineResultChosen(object sender, Telegram.Bot.Args.ChosenInlineResultEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void Bot_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            if (string.IsNullOrEmpty(e.InlineQuery.Query)) return;

            string nextOffset = null;

            if (e.InlineQuery.Query.StartsWith("r"))
                nextOffset = "register";

            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058334f, // displayed result
                    Longitude = -74.25819f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058334f,
                        Longitude = -74.25819f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.AnswerInlineQueryAsync(e.InlineQuery.Id, results, 
                isPersonal: true, cacheTime: 0);
        }

        private async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data.ToLower() == "alert")
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "رای گیری شروع می شود", showAlert: true);
            else
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "رای گیری شروع می شود", showAlert: false);

            if (e.CallbackQuery.Data.ToLower() == "accept")
            {
                await Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId,
                    "آیا **مطمئن** هستی؟", ParseMode.Markdown, false,
                    new InlineKeyboardMarkup()
                    {
                        InlineKeyboard = new InlineKeyboardButton[][]
                        {
                            new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "بلی",
                                    CallbackData = "Yes",
                                    SwitchInlineQuery = string.Empty,
                                    Url = string.Empty
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "خیر",
                                    CallbackData = "No",
                                    SwitchInlineQuery = string.Empty,
                                    Url = string.Empty
                                }
                            }
                        }
                    });
            }
            else
            {
                await Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId,
                    "نام کاربر", ParseMode.Html, false,
                    new InlineKeyboardMarkup()
                    {
                        InlineKeyboard = new InlineKeyboardButton[][]
                        {
                            new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "موافقم",
                                    CallbackData = "Accept",
                                    SwitchInlineQuery = string.Empty,
                                    Url = string.Empty
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "مخالف",
                                    CallbackData = "Denid",
                                    SwitchInlineQuery = string.Empty,
                                    Url = string.Empty
                                }
                            }
                        }
                    });
            }
        }
        protected async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var command = e.Message.Text?.ToLower()?.Replace("/", "");

            if (e.Message.Chat.Type == ChatType.Private) // sent from a private user
            {
                if (e.Message.Type == MessageType.TextMessage)
                {
                    switch (command)
                    {
                        case "start":
                            await Bot.SendTextMessageAsync(e.Message.Chat.Id, "Please choose your option:", false, false, 0, CommonReplyKeyboard);
                            //Bot.SendTextMessageAsync(e.Message.From.Id, "Please add me to a group");
                            break;
                        case "register":
                            //await Bot.SendTextMessageAsync(e.Message.From.Id, "Please add me to a group");
                            await Bot.SendTextMessageAsync(e.Message.From.Id, "TEST", false, false, 0, VoteInlineKeyboard);
                            break;
                        case "change language":
                            CurrentCulture = new CultureInfo("fa");
                            InitKeyboards();
                            break;
                        default:
                            await Bot.SendTextMessageAsync(e.Message.From.Id,
                                $"{e.Message.From.FirstName} {e.Message.From.LastName} your id is: {e.Message.From.Id}");
                            break;
                    }

                }
                else if (e.Message.Type == MessageType.ServiceMessage)
                {
                    if (e.Message.Chat.Type == ChatType.Group || e.Message.Chat.Type == ChatType.Supergroup && e.Message.Text == null)
                    {
                        Group = e.Message.Chat;
                    }
                }
            }
            else if (e.Message.Chat.Type == ChatType.Group || e.Message.Chat.Type == ChatType.Supergroup) // sent from group
            {
                if (e.Message.Type == MessageType.TextMessage && e.Message.Text.StartsWith("/"))
                {
                    switch (e.Message.Text.ToLower())
                    {
                        case "/start":
                            await Bot.SendTextMessageAsync(e.Message.From.Id, "TEST", false, false, 0, CommonReplyKeyboard);
                            //Bot.SendTextMessageAsync(e.Message.From.Id, "Please add me to a group");
                            break;
                        case "/register":
                            //await Bot.SendTextMessageAsync(e.Message.From.Id, "Please add me to a group");
                            await Bot.SendTextMessageAsync(e.Message.From.Id, "TEST", false, false, 0, VoteInlineKeyboard);
                            break;
                        default:
                            await Bot.SendTextMessageAsync(e.Message.From.Id,
                                $"{e.Message.From.FirstName} {e.Message.From.LastName} your id is: {e.Message.From.Id}");
                            break;
                    }

                }
                else if (e.Message.Type == MessageType.ServiceMessage)
                {
                    if (e.Message.Chat.Type == ChatType.Group || e.Message.Chat.Type == ChatType.Supergroup && e.Message.Text == null)
                    {
                        Group = e.Message.Chat;
                    }
                }
                else
                {
                    await Bot.SendTextMessageAsync(e.Message.From.Id, $"All commands must be started by '/'!");
                }
            }
            // else: sent from channel or etc
        }


        /// <summary>
        /// Police
        /// </summary>
        public KeyboardButton[][] GetCommonReplyKeyboard()
        {
            var commonKeyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Start"),
                    new KeyboardButton("Register"),
                    new KeyboardButton("Stop")
                },

                new KeyboardButton[]
                {
                    // to see who is alive in a game
                    new KeyboardButton("Who's alive"),
                    // ask bot in private to know your role
                    new KeyboardButton("My Role"),
                    // you will killed in current game and after that you could register in another games
                    new KeyboardButton("Kill Me")
                },

                new KeyboardButton[]
                {
                    // voting without effect on players
                    new KeyboardButton("Ineffective Voting"),
                    // last voting for kill in day
                    new KeyboardButton("Final Voting")
                },

                new KeyboardButton[]
                {
                    new KeyboardButton("Change Language"),
                    new KeyboardButton("Help"),
                    new KeyboardButton("About")
                }
            };

            return commonKeyboard;
        }

        /// <summary>
        /// Police Doctor
        /// </summary>
        public KeyboardButton[][] GetDoctorReplyKeyboard()
        {
            var doctorKeyboard = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Healing to a player")
                }
            };
            doctorKeyboard.AddRange(GetCommonReplyKeyboard());

            return doctorKeyboard.ToArray();
        }

        /// <summary>
        /// Police Detective
        /// </summary>
        public KeyboardButton[][] GetDetectiveReplyKeyboard()
        {
            var detectiveKeyboard = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("In what is role of?")
                }
            };
            detectiveKeyboard.AddRange(GetCommonReplyKeyboard());

            return detectiveKeyboard.ToArray();
        }

        /// <summary>
        /// Mafia
        /// </summary>
        public KeyboardButton[][] GetMafiaReplyKeyboard()
        {
            var mafiaKeyboard = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Kill")
                }
            };
            mafiaKeyboard.AddRange(GetCommonReplyKeyboard());

            return mafiaKeyboard.ToArray();
        }

        /// <summary>
        /// Mafia Godfather
        /// </summary>
        public KeyboardButton[][] GetGodfatherReplyKeyboard()
        {
            var godfatherKeyboard = new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Kill"),
                    new KeyboardButton("Change Role")
                }
            };
            godfatherKeyboard.AddRange(GetCommonReplyKeyboard());

            return godfatherKeyboard.ToArray();
        }

        /// <summary>
        /// Get inline keyboard for voting
        /// </summary>
        public InlineKeyboardButton[][] GetVoteInlineKeyboard()
        {
            var inlineKeys = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton()
                    {
                        Text = Properties.Resources.ResourceManager.GetString("Mafia", CurrentCulture) ,
                        CallbackData = "Alert",
                        SwitchInlineQuery = string.Empty,
                        Url = string.Empty
                    },
                    new InlineKeyboardButton()
                    {
                        Text = "CallbackData",
                        CallbackData = "NoAlert",
                        SwitchInlineQuery = string.Empty,
                        Url = string.Empty
                    }
                },
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "SwitchInlineQueryCurrentChat",
                        CallbackData = string.Empty,
                        SwitchInlineQuery = string.Empty,
                        Url = string.Empty
                    },
                    new InlineKeyboardButton
                    {
                        Text = "SwitchInlineQuery",
                        SwitchInlineQuery = "/register",
                        Url = string.Empty,
                        CallbackData = string.Empty
                    }
                },
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "Url",
                        CallbackData = string.Empty,
                        SwitchInlineQuery = string.Empty,
                        Url = "http://dualp.ir"
                    }
                }
            };

            return inlineKeys;
        }

        public void InitKeyboards()
        {
            CommonReplyKeyboard = new ReplyKeyboardMarkup(GetCommonReplyKeyboard(), true);
            DoctorReplyKeyboard = new ReplyKeyboardMarkup(GetDoctorReplyKeyboard(), true);
            DetectiveReplyKeyboard = new ReplyKeyboardMarkup(GetDetectiveReplyKeyboard(), true);
            MafiaReplyKeyboard = new ReplyKeyboardMarkup(GetMafiaReplyKeyboard(), true);
            GodfatherReplyKeyboard = new ReplyKeyboardMarkup(GetGodfatherReplyKeyboard(), true);

            VoteInlineKeyboard = new InlineKeyboardMarkup(GetVoteInlineKeyboard());
        }

        #endregion

    }
}

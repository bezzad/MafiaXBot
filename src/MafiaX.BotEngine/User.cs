namespace MafiaX.BotEngine
{
    public class User
    {
        public long Id { get; }
        public string Username { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Culture { get; set; }

        public User(long id, string username, string firstName, string lastName)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Culture = "en";
        }
        public User(long id, string username)
            : this(id, username, null, null)
        { }
        public User(Telegram.Bot.Types.User user)
            : this(user.Id, user.Username, user.FirstName, user.LastName)
        { }


        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user == null) return false;

            return user.Id == Id && user.Username == Username;
        }
        public override int GetHashCode()
        {
            return (int)Id ^ Username.GetHashCode();
        }
    }
}
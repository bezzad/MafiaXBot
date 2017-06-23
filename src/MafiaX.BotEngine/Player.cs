namespace MafiaX.BotEngine
{
    public class Player
    {
        public bool IsAlive { get; protected set; }
        public User User { get; }
        public long Id => User.Id;

        public Player(User user)
        {
            User = user;
            IsAlive = true;
        }

        public void KillMe()
        {
            IsAlive = false;
        }

        public override bool Equals(object obj)
        {
            var player = obj as Player;
            if (player == null) return false;

            return ReferenceEquals(this, obj) || Equals(User, player.User);
        }

        public override int GetHashCode()
        {
            return User.GetHashCode();
        }
    }
}

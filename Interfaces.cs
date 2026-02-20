using Avalonia.Controls;

namespace BattleCity
{
    public interface IGameObject
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        bool IsActive { get; set; }
        void Update();
        void Draw(Canvas canvas);
    }
    
    public interface IMovable
    {
        int Speed { get; set; }
        int Direction { get; set; }
        void Move(int direction, Map map);
    }
    
    public interface IDamageable
    {
        int Lives { get; set; }
        bool IsDestroyed { get; set; }
        void TakeDamage(int damage);
    }
}
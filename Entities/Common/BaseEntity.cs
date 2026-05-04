namespace Entities.Common
{
    /// <summary>
    /// har kelasi interface IEntity ra ers bary konad ( implement konad) table an dar 
    /// database sakhte mishavad
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// agar classi az IEntity ers bary kard va type ham pass dad id table dar database in type ra dashte bashad
    /// </summary>
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }

    /// <summary>
    /// classe base entity ra abstract sakhtim ke natavan az an new kard 
    /// yek type Id be shekle generic migirad ke mitavan now id ra Guid va ya int va... tarif kard
    /// </summary>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// be shekle default agar faghat az BaseEntity inherit shavad va type pass dade nashavad type int khahad bood 
    /// </summary>
    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}

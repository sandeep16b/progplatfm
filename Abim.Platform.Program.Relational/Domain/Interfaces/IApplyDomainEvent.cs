namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// IApplyDomainEvent interface
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public interface IApplyDomainEvent<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Applies the specified event.
        /// </summary>
        /// <param name="event">The event.</param>
        void Apply(TEvent @event);
    }
}

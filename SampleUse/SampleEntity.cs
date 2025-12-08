using System.ComponentModel.DataAnnotations;

namespace SampleUse;

/// <summary>
/// Represents an entity with a unique identifier, a reference, and a name.
/// </summary>
/// <remarks>Use this class to model domain objects that require identification and referencing within the system.
/// The properties provide access to the entity's core attributes.</remarks>
public class SampleEntity
{
    [Key]
    public EntityID Id { get; set; }
    public ReferenceID Reference { get; set; }
    public NameString Name { get; set; }
    public DescriptionString? Description { get; set; }
}

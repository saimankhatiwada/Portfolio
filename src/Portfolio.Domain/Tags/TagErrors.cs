using Portfolio.Domain.Abstractions;

namespace Portfolio.Domain.Tags;

public static class TagErrors
{
    public static readonly Error UserNotFound = new(
        "Tag.UserNotFound",
        "The user trying to add tag does not exists");

    public static readonly Error NotFound = new(
        "Tag.NotFound",
        "The tag with specified identifier was not found");

    public static readonly Error Conflict = new(
        "Tag.Conflict",
        "The tag with provided name already exists.");
}

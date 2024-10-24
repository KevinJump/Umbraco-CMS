using System.Collections;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Models.Membership.Permissions;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.Models.Membership;

/// <summary>
///     Represents a Group for a Backoffice User
/// </summary>
[Serializable]
[DataContract(IsReference = true)]
public class UserGroup : EntityBase, IUserGroup, IReadOnlyUserGroup
{
    // Custom comparer for enumerable
    private static readonly DelegateEqualityComparer<IEnumerable<string>> _stringEnumerableComparer =
        new(
            (enum1, enum2) => enum1.UnsortedSequenceEqual(enum2),
            enum1 => enum1.GetHashCode());

    private static readonly DelegateEqualityComparer<ISet<IGranularPermission>> _granularPermissionSetComparer =
        new(
            (set1, set2) => Equals(set1, set2),
            set => set.GetHashCode());


    private readonly IShortStringHelper _shortStringHelper;
    private string _alias;
    private string? _icon;
    private string _name;
    private bool _hasAccessToAllLanguages;
    private ISet<string> _permissions;
    private ISet<IGranularPermission> _granularPermissions;
    private List<string> _sectionCollection;
    private List<int> _languageCollection;
    private int? _startContentId;
    private int? _startMediaId;

    /// <summary>
    ///     Constructor to create a new user group
    /// </summary>
    public UserGroup(IShortStringHelper shortStringHelper)
    {
        _alias = string.Empty;
        _name = string.Empty;
        _shortStringHelper = shortStringHelper;
        _sectionCollection = new List<string>();
        _languageCollection = new List<int>();
        _permissions = new HashSet<string>();
        _granularPermissions = new HashSet<IGranularPermission>();
    }

    /// <summary>
    ///     Constructor to create an existing user group
    /// </summary>
    /// <param name="userCount"></param>
    /// <param name="alias"></param>
    /// <param name="name"></param>
    /// <param name="icon"></param>
    /// <param name="shortStringHelper"></param>
    public UserGroup(
        IShortStringHelper shortStringHelper,
        int userCount,
        string? alias,
        string? name,
        string? icon)
        : this(shortStringHelper)
    {
        UserCount = userCount;
        _alias = alias ?? string.Empty;
        _name = name ?? string.Empty;
        _icon = icon;
    }

    [DataMember]
    public int? StartMediaId
    {
        get => _startMediaId;
        set => SetPropertyValueAndDetectChanges(value, ref _startMediaId, nameof(StartMediaId));
    }

    [DataMember]
    public int? StartContentId
    {
        get => _startContentId;
        set => SetPropertyValueAndDetectChanges(value, ref _startContentId, nameof(StartContentId));
    }

    [DataMember]
    public string? Icon
    {
        get => _icon;
        set => SetPropertyValueAndDetectChanges(value, ref _icon, nameof(Icon));
    }

    [DataMember]
    public string Alias
    {
        get => _alias;
        set => SetPropertyValueAndDetectChanges(
            value.ToCleanString(_shortStringHelper, CleanStringType.Alias | CleanStringType.UmbracoCase), ref _alias!,
            nameof(Alias));
    }

    [DataMember]
    public string? Name
    {
        get => _name;
        set => SetPropertyValueAndDetectChanges(value, ref _name!, nameof(Name));
    }

    [DataMember]
    public bool HasAccessToAllLanguages
    {
        get => _hasAccessToAllLanguages;
        set => SetPropertyValueAndDetectChanges(value, ref _hasAccessToAllLanguages, nameof(HasAccessToAllLanguages));
    }

    /// <inheritdoc />
    public ISet<string> Permissions
    {
        get => _permissions;
        set => SetPropertyValueAndDetectChanges(value, ref _permissions!, nameof(Permissions), _stringEnumerableComparer);
    }

    public ISet<IGranularPermission> GranularPermissions
    {
        get => _granularPermissions;
        set => SetPropertyValueAndDetectChanges(value, ref _granularPermissions!, nameof(GranularPermissions), _granularPermissionSetComparer);
    }


    public IEnumerable<string> AllowedSections => _sectionCollection;

    public int UserCount { get; }

    public void RemoveAllowedSection(string sectionAlias)
    {
        if (_sectionCollection.Contains(sectionAlias))
        {
            _sectionCollection.Remove(sectionAlias);
        }
    }

    public void AddAllowedSection(string sectionAlias)
    {
        if (_sectionCollection.Contains(sectionAlias) == false)
        {
            _sectionCollection.Add(sectionAlias);
        }
    }

    public IEnumerable<int> AllowedLanguages
    {
        get => _languageCollection;
    }

    public void RemoveAllowedLanguage(int languageId)
    {
        if (_languageCollection.Contains(languageId))
        {
            _languageCollection.Remove(languageId);
        }
    }

    public void AddAllowedLanguage(int languageId)
    {
        if (_languageCollection.Contains(languageId) == false)
        {
            _languageCollection.Add(languageId);
        }
    }

    public void ClearAllowedLanguages() => _languageCollection.Clear();

    public void ClearAllowedSections() => _sectionCollection.Clear();

    protected override void PerformDeepClone(object clone)
    {
        base.PerformDeepClone(clone);

        var clonedEntity = (UserGroup)clone;

        // manually clone the start node props
        clonedEntity._sectionCollection = new List<string>(_sectionCollection);
    }
}

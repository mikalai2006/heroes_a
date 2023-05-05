using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EntityBook
{
    public DataBook Data = new DataBook();
    public PaginationBook Pagination;
    private TypeSchoolMagic _typeSchoolMagic = TypeSchoolMagic.AllSchools;
    public TypeSchoolMagic ActiveSchoolMagic => _typeSchoolMagic;
    private TypeSpell _typeSpell;
    public TypeSpell ActiveTypeSpell => _typeSpell;
    private List<ScriptableAttributeSpell> _activeSpells;
    public List<ScriptableAttributeSpell> ActiveSpells => _activeSpells;

    private const int LIMITPERPAGE = 18;

    public EntityBook(EntityHero hero)
    {
        Data.idPlayer = hero.Data.idPlayer;
        Data.Spells = new List<ScriptableAttributeSpell>();

        if (hero.Data.spells != null && hero.Data.spells.Count != 0)
        {
            foreach (var spell in hero.Data.spells)
            {
                var spellData = ResourceSystem.Instance
                    .GetAttributesByType<ScriptableAttributeSpell>(TypeAttribute.Spell)
                    .Find(t => t.idObject == spell);
                Data.Spells.Add(spellData);
            }
        }

        Pagination = new PaginationBook();
        InitPagination();
    }

    public void InitPagination()
    {
        RefreshPagination();
        Pagination.page = 0;
        Pagination.start = 0;
        Pagination.end = Pagination.total < Pagination.limit ? Pagination.total : Pagination.limit;
    }

    public void RefreshPagination()
    {
        CreateSpellsByTypeAndSchool();
        Pagination.total = ActiveSpells.Count;
        Pagination.limit = GetLimit();
    }

    public void ChangePage(int page)
    {
        Pagination.page += page;
        RefreshPagination();

        if (page > 0)
        {
            Pagination.start = Pagination.end;
            Pagination.end += Pagination.limit;
            if (Pagination.end > Pagination.total)
            {
                Pagination.end = Pagination.total;
            }
        }
        else if (page < 0)
        {
            Pagination.end = Pagination.start;
            Pagination.start = Pagination.start - Pagination.limit;
        }

    }

    public void SetSchoolMagic(TypeSchoolMagic typeSchoolMagic)
    {
        _typeSchoolMagic = typeSchoolMagic;
        InitPagination();
    }

    public void SetTypeSpell(TypeSpell typeSpell)
    {
        _typeSpell = typeSpell;
        InitPagination();
    }

    public int GetLimit()
    {
        int limit = 0;

        if (ActiveSchoolMagic != TypeSchoolMagic.AllSchools && Pagination.page == 0)
        {
            limit = LIMITPERPAGE - 2;
        }
        else
        {
            limit = LIMITPERPAGE;
        };

        return limit;
    }

    private void CreateSpellsByTypeAndSchool()
    {
        var result = new List<ScriptableAttributeSpell>();

        foreach (var spell in Data.Spells)
        {
            if (spell.typeSpell != ActiveTypeSpell) continue;
            if (spell.SchoolMagic.typeSchoolMagic != ActiveSchoolMagic && ActiveSchoolMagic != TypeSchoolMagic.AllSchools) continue;

            result.Add(spell);
        }

        _activeSpells = result.OrderBy(t => t.level).ToList();
    }

    public override string ToString()
    {
        return "SpellBook:::" +
            "ActiveSchoolMagic=" + ActiveSchoolMagic + ",\n" +
            "ActiveTypeSpell " + ActiveTypeSpell + " \n" +
            "Pagination.start=" + Pagination.start + ",\n" +
            "Pagination.end=" + Pagination.end + ",\n" +
            "Pagination.limit=" + Pagination.limit + ",\n" +
            "Pagination.page=" + Pagination.page + ",\n" +
            "Pagination.total=" + Pagination.total + ",\n" +
            "Pagination.totalPage=" + Pagination.totalPage;
    }
}

[System.Serializable]
public struct PaginationBook
{
    public int start;
    public int end;
    public int limit;
    public int page;
    public int totalPage;
    public int total;
}

[System.Serializable]
public struct DataBook
{
    public int idPlayer;
    public List<ScriptableAttributeSpell> Spells;
}
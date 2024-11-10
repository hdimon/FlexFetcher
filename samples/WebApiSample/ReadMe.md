# Filter GET:
**Full form:** https://localhost:7246/FlexFetcher/Filter?Filters={"Logic":"And","Filters":[{"Operator":"Neq","Field":"Address","Value":null,"Logic":null,"Filters":null},{"Operator":"Eq","Field":"Address.Town","Value":"New York","Logic":null,"Filters":null}]}

**Short form:** https://localhost:7246/FlexFetcher/Filter?Filter={"L":"And","Fs":[{"O":"Neq","F":"Address","V":null,"L":null,"Fs":null},{"O":"Eq","F":"Address.Town","V":"New York","L":null,"Fs":null}]}

# Filter POST:
**Full form:** https://localhost:7246/FlexFetcher/Filter

Body (json type):
```json
{
  "Filters": {
    "Logic": "and",
    "Filters": [
      {
        "Operator": "neq",
        "Field": "Address",
        "Value": null
      },
      {
        "Operator": "eq",
        "Field": "Address.Town",
        "Value": "New York"
      }
    ]
  }
}
```

**Short form:** https://localhost:7246/FlexFetcher/Filter

Body (json type):
```json
{
  "F": {
    "L": "and",
    "Fs": [
      {
        "O": "neq",
        "F": "Address",
        "V": null
      },
      {
        "O": "eq",
        "F": "Address.Town",
        "V": "New York"
      }
    ]
  }
}
```

# Sorter GET:
**Full form:** https://localhost:7246/FlexFetcher/Sorter?Sorters={"Sorters":[{"Field":"Surname","Direction":"Asc"},{"Field":"Id","Direction":"Asc"}]}

**Short form:** https://localhost:7246/FlexFetcher/Sorter?Sorters={"S":[{"F":"Surname","D":"Asc"},{"F":"Id","D":"Asc"}]}

# Sorter POST:
**Full form:** https://localhost:7246/FlexFetcher/Sorter

Body (json type):
```json
{
  "Sorters": {
    "Sorters": [
      {
        "Field": "Surname",
        "Direction": "Asc"
      },
      {
        "Field": "Id",
        "Direction": "Asc"
      }
    ]
  }
}
```

**Short form:** https://localhost:7246/FlexFetcher/Sorter

Body (json type):
```json
{
  "S": {
    "S": [
      {
        "F": "Surname",
        "D": "Asc"
      },
      {
        "F": "Id",
        "D": "Asc"
      }
    ]
  }
}
```

# Pager GET:
**Full form:** https://localhost:7246/FlexFetcher/Pager?Pager={"Page":1,"PageSize":2}

**Short form:** https://localhost:7246/FlexFetcher/Pager?Pager={"P":1,"Ps":2}

# Pager POST:
**Full form:** https://localhost:7246/FlexFetcher/Pager

Body (json type):
```json
{
  "Pager": {
    "Page": 1,
    "PageSize": 2
  }
}
```

**Short form:** https://localhost:7246/FlexFetcher/Pager

Body (json type):
```json
{
  "P": {
    "P": 1,
    "Ps": 2
  }
}
```

# Filter, Sorter, Pager GET:
**Full form:** https://localhost:7246/FlexFetcher?Filters={"Logic":"And","Filters":[{"Operator":"Neq","Field":"Address","Value":null,"Logic":null,"Filters":null},{"Operator":"Eq","Field":"Address.City","Value":"New York","Logic":null,"Filters":null}]}&Sorters={"Sorters":[{"Field":"Surname","Direction":"Asc"},{"Field":"Id","Direction":"Asc"}]}&Pager={"Page":1,"PageSize":2}

**Short form:** https://localhost:7246/FlexFetcher?Filter={"L":"And","Fs":[{"O":"Neq","F":"Address","V":null,"L":null,"Fs":null},{"O":"Eq","F":"Address.City","V":"New York","L":null,"Fs":null}]}&Sorters={"S":[{"F":"Surname","D":"Asc"},{"F":"Id","D":"Asc"}]}&Pager={"P":1,"Ps":2}

# Filter, Sorter, Pager POST:
**Full form:** https://localhost:7246/FlexFetcher

Body (json type):
```json
{
  "Filters": {
    "Logic": "and",
    "Filters": [
      {
        "Operator": "neq",
        "Field": "Address",
        "Value": null
      },
      {
        "Operator": "eq",
        "Field": "Address.Town",
        "Value": "New York"
      }
    ]
  },
  "Sorters": {
    "Sorters": [
      {
        "Field": "Surname",
        "Direction": "Asc"
      },
      {
        "Field": "Id",
        "Direction": "Asc"
      }
    ]
  },
  "Pager": {
    "Page": 1,
    "PageSize": 2
  }
}
```

**Short form:** https://localhost:7246/FlexFetcher

Body (json type):
```json
{
  "F": {
    "L": "and",
    "Fs": [
      {
        "O": "neq",
        "F": "Address",
        "V": null
      },
      {
        "O": "eq",
        "F": "Address.Town",
        "V": "New York"
      }
    ]
  },
  "S": {
    "S": [
      {
        "F": "Surname",
        "D": "Asc"
      },
      {
        "F": "Id",
        "D": "Asc"
      }
    ]
  },
  "P": {
    "P": 1,
    "Ps": 2
  }
}
```
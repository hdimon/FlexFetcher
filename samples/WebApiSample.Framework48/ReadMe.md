# Filter GET:
https://localhost:44381/api/FlexFetcher/GetFilter?Filters={"Logic":"And","Filters":[{"Operator":"Neq","Field":"Address","Value":null,"Logic":null,"Filters":null},{"Operator":"Eq","Field":"Address.Town","Value":"New York","Logic":null,"Filters":null}]}

# Filter POST:
https://localhost:44381/api/FlexFetcher/PostFilter

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

# Sorter GET:
https://localhost:44381/api/FlexFetcher/GetSort?Sorters={"Sorters":[{"Field":"Surname","Direction":"Asc"},{"Field":"Id","Direction":"Asc"}]}

# Sorter POST:
https://localhost:44381/api/FlexFetcher/PostSort

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

# Pager GET:
https://localhost:44381/api/FlexFetcher/GetPager?Pager={"Page":1,"PageSize":2}

# Pager POST:
https://localhost:44381/api/FlexFetcher/PostPager

Body (json type):
```json
{
  "Pager": {
    "Page": 1,
    "PageSize": 2
  }
}
```

# Filter, Sorter, Pager GET:
https://localhost:44381/api/FlexFetcher?Filters={"Logic":"And","Filters":[{"Operator":"Neq","Field":"Address","Value":null,"Logic":null,"Filters":null},{"Operator":"Eq","Field":"Address.City","Value":"New York","Logic":null,"Filters":null}]}&Sorters={"Sorters":[{"Field":"Surname","Direction":"Asc"},{"Field":"Id","Direction":"Asc"}]}&Pager={"Page":1,"PageSize":2}

# Filter, Sorter, Pager POST:
https://localhost:44381/api/FlexFetcher

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
using FlexFetcher.Utils;
using System.Linq.Expressions;

namespace FlexFetcher.Models.FlexFetcherOptions;

public class FlexFetcherOptions<TEntity, TModel> : FlexFetcherOptions<TEntity> where TEntity : class where TModel : class
{
    public FlexFetcherOptions()
    {
        FilterOptions = new FlexFilterOptions<TEntity, TModel>();
        SorterOptions = new FlexSorterOptions<TEntity, TModel>();
    }

    public FlexFetcherOptions(FlexFilterOptions<TEntity, TModel> filterOptions)
    {
        FilterOptions = filterOptions;
        SorterOptions = new FlexSorterOptions<TEntity, TModel>();
    }

    public FlexFetcherOptions(FlexSorterOptions<TEntity, TModel> sorterOptions)
    {
        FilterOptions = new FlexFilterOptions<TEntity, TModel>();
        SorterOptions = sorterOptions;
    }

    public FlexFetcherOptions(FlexFilterOptions<TEntity, TModel> filterOptions, FlexSorterOptions<TEntity, TModel> sorterOptions)
    {
        FilterOptions = filterOptions;
        SorterOptions = sorterOptions;
    }

    public new FieldBuilder<TEntity, TField, TModel> Field<TField>(Expression<Func<TEntity, TField>> fieldExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TModel>(fieldExpression);
        FilterOptions.AddFieldBuilderInternal(builder);
        SorterOptions.AddFieldBuilderInternal(builder);

        return builder;
    }
}

public class FlexFetcherOptions<TEntity> where TEntity : class
{
    public FlexFilterOptions<TEntity> FilterOptions { get; protected set; }
    public FlexSorterOptions<TEntity> SorterOptions { get; protected set; }

    public FlexFetcherOptions()
    {
        FilterOptions = new FlexFilterOptions<TEntity>();
        SorterOptions = new FlexSorterOptions<TEntity>();
    }

    public FlexFetcherOptions(FlexFilterOptions<TEntity> filterOptions)
    {
        FilterOptions = filterOptions;
        SorterOptions = new FlexSorterOptions<TEntity>();
    }

    public FlexFetcherOptions(FlexSorterOptions<TEntity> sorterOptions)
    {
        FilterOptions = new FlexFilterOptions<TEntity>();
        SorterOptions = sorterOptions;
    }

    public FlexFetcherOptions(FlexFilterOptions<TEntity> filterOptions, FlexSorterOptions<TEntity> sorterOptions)
    {
        FilterOptions = filterOptions;
        SorterOptions = sorterOptions;
    }

    public CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>> AddCustomField(IFlexCustomField<TEntity> customField)
    {
        var builder = new CustomFieldBuilder<TEntity, IFlexCustomField<TEntity>>(customField);
        FilterOptions.AddFieldBuilderInternal(builder);
        SorterOptions.AddFieldBuilderInternal(builder);

        FilterOptions.AddCustomFieldInternal(customField);
        SorterOptions.AddCustomFieldInternal(customField);

        return builder;
    }

    public virtual FieldBuilder<TEntity, TField, TEntity> Field<TField>(Expression<Func<TEntity, TField>> fieldExpression)
    {
        var builder = new FieldBuilder<TEntity, TField, TEntity>(fieldExpression);
        FilterOptions.AddFieldBuilderInternal(builder);
        SorterOptions.AddFieldBuilderInternal(builder);

        return builder;
    }

    public void Build()
    {
        if (FilterOptions.IsBuilt && SorterOptions.IsBuilt)
            return;

        FilterOptions.Build();
        SorterOptions.Build();
    }

    /// <summary>
    /// Hides all original fields of Entity from sorting/filtering,
    /// i.e. if fields are hidden then they can be accessed only by their aliases.
    /// Field aliases are not affected by this method.
    /// </summary>
    public void HideOriginalFields()
    {
        FilterOptions.HideOriginalFields();
        SorterOptions.HideOriginalFields();
    }
}
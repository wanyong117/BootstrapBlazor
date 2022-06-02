﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Shared;
using System.Linq;

namespace UnitTest.Extensions;

public class LambadaExtensionsTest
{
    [Fact]
    public void GetFilterFunc_Null()
    {
        var foos = new Foo[]
        {
            new() { Count = 1 },
            new() { Count = 2 },
            new() { Count = 10 },
            new() { Count = 11 }
        };
        var filter = Array.Empty<MockFilterActionBase>();
        var items = foos.Where(filter.GetFilterFunc<Foo>());
        Assert.Equal(4, items.Count());
    }

    [Fact]
    public void GetFilterFunc_Ok()
    {
        var filter = new FilterKeyValueAction();
        var invoker = filter.GetFilterFunc<Foo>();
        Assert.True(invoker(new Foo()));
    }

    [Fact]
    public void GetFilterLambda_Nullable()
    {
        var filters = new FilterKeyValueAction() { FieldKey = nameof(Foo.DateTime), FieldValue = DateTime.MinValue };
        var exp = filters.GetFilterLambda<Foo>();
        Assert.True(exp.Compile().Invoke(new Foo() { DateTime = DateTime.MinValue }));
    }

    [Fact]
    public void GetFilterLambda_Enum()
    {
        var filters = new FilterKeyValueAction() { FieldKey = nameof(Dummy.Education), FieldValue = "Middel" };
        var exp = filters.GetFilterLambda<Dummy>();
        Assert.True(exp.Compile().Invoke(new Dummy() { Education = EnumEducation.Middel }));
    }

    [Fact]
    public void GetFilterLambda_Null()
    {
        var filters = Array.Empty<FilterKeyValueAction>();
        var exp = filters.GetFilterLambda<Foo>();
        Assert.True(exp.Compile().Invoke(new Foo()));
    }

    [Fact]
    public void GetFilterLambda_And()
    {
        var foos = new Foo[]
        {
            new() { Count = 1 },
            new() { Count = 2 },
            new() { Count = 10 },
            new() { Count = 11 }
        };
        var filter = new MockFilterActionBase[]
        {
            new MockAndFilterAction1(),
            new MockAndFilterAction2()
        };
        var items = foos.Where(filter.GetFilterFunc<Foo>());
        Assert.Single(items);
    }

    [Fact]
    public void GetFilterLambda_Or()
    {
        var foos = new Foo[]
        {
            new() { Count = 1 },
            new() { Count = 2 },
            new() { Count = 10 },
            new() { Count = 11 }
        };
        var filter = new MockFilterActionBase[]
        {
            new MockOrFilterAction1(),
            new MockOrFilterAction2()
        };
        var items = foos.Where(filter.GetFilterFunc<Foo>(FilterLogic.Or));
        Assert.Equal(3, items.Count());
    }

    [Fact]
    public void FilterKeyValueAction_FieldKey_Null()
    {
        var filter = new FilterKeyValueAction();
        var invoker = filter.GetFilterLambda<Foo>().Compile();
        Assert.True(invoker.Invoke(new Foo()));
    }

    [Fact]
    public void FilterKeyValueAction_SimpleFilterExpression()
    {
        var filter = new FilterKeyValueAction() { FieldKey = "Name", FieldValue = "Name" };
        var invoker = filter.GetFilterLambda<Foo>().Compile();
        Assert.True(invoker.Invoke(new Foo() { Name = "Name" }));
    }

    [Fact]
    public void FilterKeyValueAction_SimpleFilterExpression_Exception()
    {
        var filter = new FilterKeyValueAction() { FieldKey = "Name", FieldValue = "Name" };
        Assert.Throws<InvalidOperationException>(() => filter.GetFilterLambda<Dummy>());
    }

    [Fact]
    public void FilterKeyValueAction_ComplexFilterExpression()
    {
        var filter = new FilterKeyValueAction() { FieldKey = "Foo.Name", FieldValue = "Name" };
        var invoker = filter.GetFilterLambda<Dummy>().Compile();
        Assert.True(invoker.Invoke(new Dummy() { Foo = new Foo() { Name = "Name" } }));
    }

    [Fact]
    public void FilterKeyValueAction_ComplexFilterExpression_Exception()
    {
        var filter = new FilterKeyValueAction() { FieldKey = "Foo.TestName", FieldValue = "Name" };
        Assert.Throws<InvalidOperationException>(() => filter.GetFilterLambda<Dummy>());

        filter = new FilterKeyValueAction() { FieldKey = "Foo1.TestName", FieldValue = "Name" };
        Assert.Throws<InvalidOperationException>(() => filter.GetFilterLambda<Dummy>());
    }

    private abstract class MockFilterActionBase : IFilterAction
    {
        public abstract IEnumerable<FilterKeyValueAction> GetFilterConditions();

        public virtual void Reset()
        {
        }

        public virtual Task SetFilterConditionsAsync(IEnumerable<FilterKeyValueAction> conditions)
        {
            return Task.CompletedTask;
        }
    }

    private class MockAndFilterAction1 : MockFilterActionBase
    {
        public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
        {
            var filters = new FilterKeyValueAction[]
            {
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 1,
                     FilterAction = FilterAction.GreaterThan,
                     FilterLogic = FilterLogic.And
                },
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 10,
                     FilterAction = FilterAction.LessThan
                }
            };
            return filters;
        }
    }

    private class MockAndFilterAction2 : MockFilterActionBase
    {
        public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
        {
            var filters = new FilterKeyValueAction[]
            {
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 2,
                     FilterAction = FilterAction.Equal,
                     FilterLogic = FilterLogic.And
                }
            };
            return filters;
        }
    }

    private class MockOrFilterAction1 : MockFilterActionBase
    {
        public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
        {
            var filters = new FilterKeyValueAction[]
            {
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 1,
                     FilterAction = FilterAction.Equal,
                     FilterLogic = FilterLogic.Or
                },
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 2,
                     FilterAction = FilterAction.Equal,
                     FilterLogic = FilterLogic.Or
                }
            };
            return filters;
        }
    }

    private class MockOrFilterAction2 : MockFilterActionBase
    {
        public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
        {
            var filters = new FilterKeyValueAction[]
            {
                new()
                {
                     FieldKey = "Count",
                     FieldValue = 10,
                     FilterAction = FilterAction.Equal
                }
            };
            return filters;
        }
    }

    private class Dummy
    {
        public Foo? Foo { get; set; }

        public EnumEducation Education { get; set; }
    }
}

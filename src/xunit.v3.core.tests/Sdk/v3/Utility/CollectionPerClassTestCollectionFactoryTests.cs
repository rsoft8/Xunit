﻿using System.Linq;
using Xunit;
using Xunit.v3;

public class CollectionPerClassTestCollectionFactoryTests
{
	[Fact]
	public static void DefaultCollectionBehaviorIsCollectionPerClass()
	{
		var type1 = Mocks.TypeInfo("FullyQualified.Type.Number1");
		var type2 = Mocks.TypeInfo("FullyQualified.Type.Number2");
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll");
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		var result1 = factory.Get(type1);
		var result2 = factory.Get(type2);

		Assert.NotSame(result1, result2);
		Assert.Equal("Test collection for FullyQualified.Type.Number1", result1.DisplayName);
		Assert.Equal("Test collection for FullyQualified.Type.Number2", result2.DisplayName);
		Assert.Null(result1.CollectionDefinition);
		Assert.Null(result2.CollectionDefinition);
	}

	[Fact]
	public static void ClassesDecoratedWithSameCollectionNameAreInSameTestCollection()
	{
		var attr = Mocks.CollectionAttribute("My Collection");
		var type1 = Mocks.TypeInfo("type1", attributes: new[] { attr });
		var type2 = Mocks.TypeInfo("type2", attributes: new[] { attr });
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll");
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		var result1 = factory.Get(type1);
		var result2 = factory.Get(type2);

		Assert.Same(result1, result2);
		Assert.Equal("My Collection", result1.DisplayName);
	}

	[Fact]
	public static void ClassesWithDifferentCollectionNamesHaveDifferentCollectionObjects()
	{
		var type1 = Mocks.TypeInfo("type1", attributes: new[] { Mocks.CollectionAttribute("Collection 1") });
		var type2 = Mocks.TypeInfo("type2", attributes: new[] { Mocks.CollectionAttribute("Collection 2") });
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll");
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		var result1 = factory.Get(type1);
		var result2 = factory.Get(type2);

		Assert.NotSame(result1, result2);
		Assert.Equal("Collection 1", result1.DisplayName);
		Assert.Equal("Collection 2", result2.DisplayName);
	}

	[Fact]
	public static void ExplicitlySpecifyingACollectionWithTheSameNameAsAnImplicitWorks()
	{
		var type1 = Mocks.TypeInfo("type1");
		var type2 = Mocks.TypeInfo("type2", attributes: new[] { Mocks.CollectionAttribute("Test collection for type1") });
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll");
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		var result1 = factory.Get(type1);
		var result2 = factory.Get(type2);

		Assert.Same(result1, result2);
		Assert.Equal("Test collection for type1", result1.DisplayName);
	}

	[Fact]
	public static void UsingTestCollectionDefinitionSetsTypeInfo()
	{
		var testType = Mocks.TypeInfo("type", attributes: new[] { Mocks.CollectionAttribute("This is a test collection") });
		var collectionDefinitionType = Mocks.TypeInfo("collectionDefinition", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll", types: new[] { collectionDefinitionType });
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		var result = factory.Get(testType);

		Assert.Same(collectionDefinitionType, result.CollectionDefinition);
	}

	[Fact]
	public static void MultiplyDeclaredCollectionsRaisesEnvironmentalWarning()
	{
		var spy = SpyMessageSink.Capture();
		TestContext.Current!.DiagnosticMessageSink = spy;
		var testType = Mocks.TypeInfo("type", attributes: new[] { Mocks.CollectionAttribute("This is a test collection") });
		var collectionDefinition1 = Mocks.TypeInfo("collectionDefinition1", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
		var collectionDefinition2 = Mocks.TypeInfo("collectionDefinition2", attributes: new[] { Mocks.CollectionDefinitionAttribute("This is a test collection") });
		var assembly = Mocks.TestAssembly(@"C:\Foo\bar.dll", types: new[] { collectionDefinition1, collectionDefinition2 });
		var factory = new CollectionPerClassTestCollectionFactory(assembly);

		factory.Get(testType);

		var msg = Assert.Single(spy.Messages.OfType<_DiagnosticMessage>().Select(m => m.Message));
		Assert.Equal("Multiple test collections declared with name 'This is a test collection': collectionDefinition1, collectionDefinition2", msg);
	}
}

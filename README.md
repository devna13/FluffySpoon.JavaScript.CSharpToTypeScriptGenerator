A flexible CSharp to TypeScript generator that is `Gulp` and `Grunt` friendly, written in TypeScript.

Uses the following library for parsing C# code from TypeScript: https://github.com/ffMathy/FluffySpoon.JavaScript.CSharpParser

# Wrappers for build runners
- **Gulp:** https://github.com/ffMathy/FluffySpoon.JavaScript.CSharpToTypeScriptGenerator.Gulp
- **Grunt:** https://github.com/ffMathy/FluffySpoon.JavaScript.CSharpToTypeScriptGenerator.Grunt

# Usage examples
These recipes help you quickly get started with common scenarios you may need. Feel free to contribute with your own!

The following code shows general usage. The examples below only differ in the `EmitOptions` provided.

```typescript
import { Emitter } from '@fluffy-spoon/javascript.csharp-to-typescript-generator';

var csharpCode = "insert the CSharp model code here - you could also read it from a file.";
var emitter = new Emitter(csharpCode);
var options = <EmitOptions>{ 
  defaults: <DefaultEmitOptions>{ },
  file: <FileEmitOptions>{ }
};
var typescriptCode = emitter.emitFile(options);
```

- To see the definitions of each C# type such as `CSharpType`, look here: https://github.com/ffMathy/FluffySpoon.JavaScript.CSharpParser/blob/master/dist/src/Models.ts
- To see the definitions of each option type such as `FileEmitOptions`, look here: https://github.com/ffMathy/FluffySpoon.JavaScript.CSharpToTypeScriptGenerator/blob/master/dist/src/Index.d.ts

## Recipes for frameworks & libraries
To see pre-made examples designed for frameworks like Angular and ASP .NET Core (for instance for auto-generating HTTP clients for each controller action), go [see the recipes here](doc/RECIPES.md).

## Default settings
```typescript
var typescriptCode = emitter.emitFile();
```

Given the following CSharp model code:

```csharp
namespace MyNamespace {
  public class MyClass {
    public bool myField;
  
    public int MyProperty { get; set; }
    public string MyOtherProperty { get; set; }
    public double? MyNullableProperty { get; set; }
    
    public class MySubclass {
      public List<string> MyListProperty { get; set; }
      public MyGenericType<SomeType, SomeOtherType> MyGenericProperty { get; set; }
      public Task MyFunction(string input1, int input2) { 
        //some code
      }
    }
  }
}
```

The following TypeScript code would be generated:

```typescript
declare namespace MyNamespace {
  interface MyClass {
    myField: boolean;
    myProperty: number;
    myOtherProperty: string;
    myNullableProperty?: number;
  }
  
  namespace MyClass {
    interface MySubclass {
      myListProperty: string[];
      myGenericProperty: MyGenericType<SomeType, SomeOtherType>;
      MyFunction(input1: string, input2: number): Promise;
    }
  }
}
```

### Ignoring methods
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    methodEmitOptions: <MethodEmitOptions>{
      filter: (method: CSharpMethod) => false //returning false filters away all methods
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass {
  public int MyProperty { get; set; }
  public Task MyFunction(string input1, int input2) { 
    //some code
  }
}
```

The following TypeScript code would be generated:

```typescript
declare interface MyClass {
  myProperty: number;
}
```

## Wrapping all emitted code in a namespace
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  file: <FileEmitOptions>{
    onAfterParse: (file: CSharpFile) => {
      //we create a namespace, move all items of the file into that namespace, and remove the same items from the file. 
      //we then add the newly created namespace to the file.

      var namespace = new CSharpNamespace("MyNamespace");
      namespace.classes = file.classes;
      namespace.enums = file.enums;
      namespace.innerScopeText = file.innerScopeText;
      namespace.interfaces = file.interfaces;
      namespace.namespaces = file.namespaces;
      namespace.parent = file;
      namespace.structs = file.structs;
      namespace.usings = file.usings;

      file.classes = [];
      file.enums = [];
      file.interfaces = [];
      file.namespaces = [];
      file.structs = [];
      file.usings = [];

      file.namespaces.push(namespace);
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass {
  public int MyProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare namespace MyNamespace {
  interface MyClass {
    myProperty: number;
  }
}
```

## Specify what TypeScript types specific CSharp types map to
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    typeEmitOptions: <TypeEmitOptions>{
      mapper: (type: CSharpType, suggested: string) => type.name === "DateTime" ? "Date" : suggested
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass {
  public DateTime MyProperty { get; set; }
  public string MyOtherProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare interface MyClass {
  myProperty: Date;
  myOtherProperty: string;
}
```

## Including private properties
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    propertyEmitOptions: <PropertyEmitOptions>{
      filter: (property: CSharpProperty) => true //the default filter is "property.isPublic === true"
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass {
  private int MyProperty { get; set; }
  public string MyOtherProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare interface MyClass {
  myProperty: number;
  myOtherProperty: string;
}
```

**Note:** This can also be done for classes, methods and fields by using the `ClassEmitOptions`, `MethodEmitOptions`, and `FieldEmitOptions` respectively.

## Pascal-casing property names
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    propertyEmitOptions: <PropertyEmitOptions>{
      perPropertyEmitOptions: (property: CSharpProperty) => <PerPropertyEmitOptions>{
        name: property.name //the default is camel casing
      }
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass {
  public int MyProperty { get; set; }
  public string MyOtherProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare interface MyClass {
  MyProperty: number;
  MyOtherProperty: string;
}
```

**Note:** This can also be done for classes, types, methods and fields by using the `ClassEmitOptions`, `TypeEmitOptions`, `MethodEmitOptions` and `FieldEmitOptions` respectively.

## Prefixing all class names with "I"
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    classEmitOptions: <ClassEmitOptions>{
      perClassEmitOptions: (classObjcect: CSharpClass) => <PerClassEmitOptions>{
        name: "I" + classObject.name,
        inheritedTypeEmitOptions: { 
          //this is needed to also change the name of the inherited class, if any
          mapper: (type, suggested) => "I" + suggested
        }
      }
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass: SomeInheritedClass {
  public int MyProperty { get; set; }
}

public class SomeInheritedClass {
  public int MyBaseProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare interface IMyClass extends ISomeInheritedClass {
  myProperty: number;
}

declare interface ISomeInheritedClass {
  myBaseProperty: number;
}
```

**Note:** This can also be done for interfaces by using the `InterfaceEmitOptions` instead.

## Removing inheritance
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <FileEmitOptions>{
    classEmitOptions: <ClassEmitOptions>{
      perClassEmitOptions: (classObjcect: CSharpClass) => <PerClassEmitOptions>{
        inheritedTypeEmitOptions: { 
          //by mapping the inherited type to "null", it is not emitted
          mapper: (type, suggested) => null
        }
      }
    }
  }
});
```

Given the following CSharp model code:

```csharp
public class MyClass: SomeInheritedClass {
  public int MyProperty { get; set; }
}

public class SomeInheritedClass {
  public int MyBaseProperty { get; set; }
}
```

The following TypeScript code would be generated:

```typescript
declare interface MyClass {
  myProperty: number;
}

declare interface SomeInheritedClass {
  myBaseProperty: number;
}
```

**Note:** This can also be done for interfaces by using the `InterfaceEmitOptions` instead.

## Convert enums to string union types
```typescript
var typescriptCode = emitter.emitFile(<EmitOptions>{
  defaults: <DefaultEmitOptions>{
    enumEmitOptions: <EnumEmitOptions>{
      strategy: "string-union"
    }
  }
});
```

Given the following CSharp model code:

```csharp
public enum MyEnum {
  FirstOption,
  SecondOption
}
```

The following TypeScript code would be generated:

```typescript
declare type MyEnum =
  'FirstOption' |
  'SecondOption'
```

# Configuration
The `EmitOptions` are the root options. They include two properties:

- `defaults` **DefaultEmitOptions**  
  makes it easy to treat specific C# constructs in a specific way per default if no other settings are explicitly specified (for instance, lower-casing all property names). This settings hieracy is flat. 

  - `classEmitOptions` **ClassEmitOptions**  
    configures default class settings. This settings hierachy is flat.  

    - `declare` **boolean**  
      determines whether or not the class should be declared (using the `declare` keyword in TypeScript).  

    - `filter` **(classObject: CSharpClass) => boolean**  
      decides (for every `CSharpClass` considered) whether or not it should be included in the emit.  

    - `perClassEmitOptions` **(classObject: CSharpClass) => PerClassEmitOptions**  

  - `namespaceEmitOptions` **NamespaceEmitOptions**  
    configures default namespace settings. This settings hierachy is flat.  

    - `declare` **boolean**  
      determines whether or not the namespace should be declared (using the `declare` keyword in TypeScript).

	  - `skip` **boolean**  
      determines whether or not the namespace should be skipped (in other words, emitting its contents directly without the enclosing namespace declaration).

	  - `filter` **(namespace: CSharpNamespace) => boolean**  
      decides (for every `CSharpNamespace` considered) whether or not it should be included in the emit.

  - `enumEmitOptions` **EnumEmitOptions**  
    configures default enum settings. This settings hierachy is flat.

    - `declare` **boolean**  

    - `strategy` **"default" | "string-union"**  

    - `filter` **(enumObject: CSharpEnum) => boolean**  

  - `structEmitOptions` **StructEmitOptions**  
    configures default struct settings. This settings hierachy is flat.  

    - `declare` **boolean**  
      determines whether or not the struct should be declared (using the `declare` keyword in TypeScript).  

    - `filter` **struct: CSharpStruct) => boolean**  
      decides (for every `CSharpStruct` considered) whether or not it should be included in the emit.

    - `perStructEmitOptions` **(struct: CSharpStruct) => PerStructEmitOptions**  

  - `interfaceEmitOptions` **InterfaceEmitOptions**  
    configures default interfaces settings. This settings hierachy is flat.  

    - `declare` **boolean**  
      determines whether or not the interface should be declared (using the `declare` keyword in TypeScript).  

    - `filter` **(method: CSharpInterface) => boolean**  
      decides (for every `CSharpInterface` considered) whether or not it should be included in the emit.  

    - `perInterfaceEmitOptions` **(interfaceObject: CSharpInterface) => PerInterfaceEmitOptions**  

  - `typeEmitOptions` **TypeEmitOptions**  
    configures default type settings. This settings hierachy is flat.  

    - `mapper` **(type: CSharpType, suggestedOutput: string) => string**  
      allows customization of types. Returns a `string` of pure TypeScript type code (for instance `number|string`). The argument `suggestedOutput` is the TypeScript type that would have been mapped per default (for instance, per default, `int` is mapped to `number`, and `suggestedOutput` would be `number` in that case).  

    - `filter` **(type: CSharpType) => boolean**  
      decides (for every `CSharpType` considered) whether or not it should be included in the emit. Skipped types often cause their parents to also be skipped. For instance, if the type of an argument is skipped, the whole argument is skipped too. Furthermore if the base type of a class is skipped, its `extends` keyword is not emitted either.  

  - `propertyEmitOptions` **PropertyEmitOptions**  
    configures default property settings. This settings hierachy is flat.  

    - `readOnly` **boolean**  
      decides if the TypeScript `readonly` keyword should be used for this property.  

    - `filter` **(property: CSharpProperty) => boolean**  
      decides (for every `CSharpProperty` considered) whether or not it should be included in the emit.  

    - `perPropertyEmitOptions` **(property: CSharpProperty) => PerPropertyEmitOptions**  

  - `fieldEmitOptions` **FieldEmitOptions**  
    configures default field settings. This settings hierachy is flat.  

    - `readOnly` **boolean**  
      decides if the TypeScript `readonly` keyword should be used for this property.  

    - `filter` **(field: CSharpField) => boolean**  
      decides (for every `CSharpField` considered) whether or not it should be included in the emit. 

    - `perFieldEmitOptions` **(field: CSharpField) => PerFieldEmitOptions**  

  - `methodEmitOptions` **MethodEmitOptions**  
    configures default method settings. This settings hierachy is flat.  

- `file` **FileEmitOptions**  
  configures file-level settings. This is a nested configuration hierachy. Every property defined here overrides the default one. This structure is recursive.  

  - `classEmitOptions` **ClassEmitOptions**  
    configures classes found within the file. 

	  - `enumEmitOptions` **EnumEmitOptions**  
      is exactly like the `enumEmitOptions` defined under `FileEmitOptions`, but only applies to the enums found within classes.  

    - `propertyEmitOptions` **PropertyEmitOptions**  
      configures properties found within the class.  

      - `typeEmitOptions` **TypeEmitOptions**  
        configures the type of the property (for instance `string` in `string Foo { get; set; }`).  

    - `interfaceEmitOptions` **InterfaceEmitOptions**  
      is exactly like the `interfaceEmitOptions` defined under `FileEmitOptions`, but only applies to the interfaces found within classes.  

    - `methodEmitOptions` **MethodEmitOptions**  
      configures methods found within the class.  

      - `returnTypeEmitOptions` **TypeEmitOptions**  
        configures the return type of the method (for instance `string` in `string DoFoo() { }`).  

      - `argumentTypeEmitOptions` **TypeEmitOptions**  
        configures the argument types of the method (for instance `string` and `int` in `void DoFoo(string arg1, int arg2) { }`).  

    - `fieldEmitOptions` **FieldEmitOptions**  
      configures fields found within the class.  

      - `typeEmitOptions` **TypeEmitOptions**  
        is exactly like the `typeEmitOptions` defined under `PropertyEmitOptions`, but applies to the type of the field instead of the property.  

    - `structEmitOptions` **StructEmitOptions**  
      is exactly like the `structEmitOptions` defined under `FileEmitOptions`, but only applies to the structs found within classes.  

    - `genericParameterTypeEmitOptions` **TypeEmitOptions**  
      configures generic type parameters of the class (for instance settings for `T` and `K` in the class `Foo<T, K>`).  

    - `inheritedTypeEmitOptions` **TypeEmitOptions**  
      configures the inherited type of the class (for instance `MyBaseClass` for the class `MyClass : MyBaseClass`).  

  - `namespaceEmitOptions` **NamespaceEmitOptions**  
    configures namespaces found within the file.  

	  - `classEmitOptions` **ClassEmitOptions**  
      is exactly like the `classEmitOptions` defined under `FileEmitOptions`, but only applies to the classes found within namespaces.  

	  - `interfaceEmitOptions` **InterfaceEmitOptions**  
      is exactly like the `interfaceEmitOptions` defined under `FileEmitOptions`, but only applies to the interfaces found within namespaces.  

	  - `structEmitOptions` **StructEmitOptions**  
      is exactly like the `structEmitOptions` defined under `FileEmitOptions`, but only applies to the structs found within namespaces.  

	  - `enumEmitOptions` **EnumEmitOptions**  
      is exactly like the `enumEmitOptions` defined under `FileEmitOptions`, but only applies to the enums found within namespaces.  

  - `enumEmitOptions` **EnumEmitOptions**  
    configures enums found within the file.  

  - `structEmitOptions` **StructEmitOptions**  
    configures structs found within the file.  

    - `propertyEmitOptions` **PropertyEmitOptions**  
      is exactly like the `propertyEmitOptions` defined under `ClassEmitOptions`, but only applies to the properties found within structs.  

    - `methodEmitOptions` **MethodEmitOptions**  
      is exactly like the `methodEmitOptions` defined under `ClassEmitOptions`, but only applies to the methods found within structs.  

    - `fieldEmitOptions` **FieldEmitOptions**  
      is exactly like the `fieldEmitOptions` defined under `ClassEmitOptions`, but only applies to the fields found within structs.  

  - `interfaceEmitOptions` **InterfaceEmitOptions**  
    configures interfaces found within the file.  

    - `propertyEmitOptions` **PropertyEmitOptions**  
      is exactly like the `propertyEmitOptions` defined under `ClassEmitOptions`, but only applies to the properties found within interfaces.  

    - `methodEmitOptions` **MethodEmitOptions**  
      is exactly like the `methodEmitOptions` defined under `ClassEmitOptions`, but only applies to the methods found within interfaces.  

    - `genericParameterTypeEmitOptions` **TypeEmitOptions**  
      is exactly like the `genericParameterTypeEmitOptions` defined under `ClassEmitOptions`, but only applies to the generic parameters of interfaces.  

    - `inheritedTypeEmitOptions` **TypeEmitOptions**  
      is exactly like the `inheritedTypeEmitOptions` defined under `ClassEmitOptions`, but only applies to implemented types of interfaces.  

	- `onAfterParse` **(file: CSharpFile) => void**  
    fires right after the C# code has been parsed, but before the TypeScript code has been emitted. `file` here contains the parsed C# code.  

	- `onBeforeEmit` **(file: CSharpFile, typeScriptEmitter: TypeScriptEmitter) => void**   
    fires right after the C# code has been parsed, but before the TypeScript code has been emitted. `file` here contains the parsed C# code, and `typeScriptEmitter` allows for more fine-grained control over the file emission (directly emitting strings into the file) compared to the `onAfterParse` variant.  

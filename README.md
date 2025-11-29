# Расширение списока правил к именованию и написанию кода

Содержит список простых правил и их исправлений для C# кода:

- именование классов в PascalCase
- именование свойств в PascalCase
- именование методов в PascalCase
- именование констант в CapitalCase
- именование приватных полей в CamelCase c _ в начале имени
- асинхронные методы должны заканчиваться на Async. За исключением публичных методов в контроллерах
- удаление необязательных скобочек для if
- Проверка, что имена содержат только английские буквы, цифры и _

## Именование классов в PascalCase

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.class_naming.severity = none
```

Пример:

```csharp
//Находит
class camelCase
{
    public camelCase() {}
    //...
}

//Переименовывет в
class CamelCase
{
    public CamelCase() {}
    //...
}
```

## Именование свойств в PascalCase

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.property_naming.severity = none
```

Пример:

```csharp
//Находит
public int MyProperty() { get; set; }
public int my_property2() { get; set; }

//Переименовывет в
public int myProperty() { get; set; }
public int myProperty2() { get; set; }
```

## Именование методов в PascalCase

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.method_naming.severity = none
```

Пример:

```csharp
//Находит
public void voidMethod()
{
    //...
}

public int get_int()
{
    //...
}

//Переименовывет в
public void VoidMethod()
{
    //...
}

public int GetInt()
{
    //...
}
```

## Именование констант в CapitalCase

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.constant_capital_case_naming.severity = none
```

Пример:

```csharp
//Находит
public const int class_constant = 1;

public void Method()
{
    const int local_constant = 2;
    //...
}

//Переименовывет в
public const int CLASS_CONSTANT = 1;

public void Method()
{
    const int LOCAL_CONSTANT = 2;
    //...
}
```

## Именование приватных полей

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.private_field_naming.severity = none
```

Пример:

```csharp
//Находит
private int Field = 1;

//Переименовывет в
private int _field = 1;
```

## Именование асинхронных методов - должны заканчиваться на Async

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.method_ends_async.severity = none
```

Пример:

```csharp
//Находит
public Task Method()
{
    //...
}

public Task<int> GetInt()
{
    //...
}

//Переименовывет в
public Task MethodAsync()
{
    //...
}

public Task<int> GetIntAsync()
{
    //...
}
```

## Удаление необязательных скобочек для if

Уровень реагирования по умолчанию: **warning**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.unnecssary_if_braces.severity = none
```

Пример:

```csharp
//Находит
var someBool = true;
var variable = 1;

if (someBool)
{
    variable = 2;
}
else
{
    variable = 3;
}

//Переименовывет в
var someBool = true;
var variable = 1;

if (someBool)
    variable = 2;
else
    variable = 3;
```

Не находит когда тело большое

```csharp
var someBool = true;
var variable = 1;

if (someBool)
{
    variable = 2;
    someBool = false;
}
```

Не находит когда if содержит большое условие

```csharp
var someBool = true;
var variable = 1;

if (someBool &&
    variable != 2)
{
    variable = 2;
}
```

## Проверка, что имена содержат только английские буквы, цифры и _

Уровень реагирования по умолчанию: **error**

Отключение в .editconfig:

```.editconfig
dotnet_diagnostic.right_naming.severity = none
```

Пример:

```csharp
//Будет выдавать ошибки на все именования
public class ИмяКласса
{
    public const int КОНСТАНТА = 1;

    public int _поле_ = 2;

    public int Свойство { get; set; } = 3;

    public void Метод()
    {
        var локальнаяПеременная = 4;

        //...
    }
}
```
# CircularStack
#### A simple stack (LIFO) that rotates items based on its fixed capacity.

[![NuGet version (CircularStack)](https://img.shields.io/nuget/v/CircularStack.svg?style=flat-square)](https://www.nuget.org/packages/CircularStack/)
[![Build status](https://ci.appveyor.com/api/projects/status/x9cv2228prqcqst9/branch/master?svg=true)](https://ci.appveyor.com/project/jpmaria/circularstack)
 
## Get Packages

You can start using CircularStack by [grabbing the latest NuGet packages](https://www.nuget.org/packages/CircularStack/).

## Usage

Having a fixed capacity means CircularStack will rotate items in the stack based on that capacity and how old they are, in fact **dropping older items when full**.

Other than having a fixed capacity, CircularStack behaves just as a normal Stack (LIFO) in what concerns to Push/Pop/Peek methods.

CircularStack can be used without specifying a type to contain, in which case it will be a `CircularStack<object>`.

CircularStack is **not** thread safe so if you need to synchronize over multiple threads, you should use  the `CircularStack.SyncRoot` object.

#### Initialization:
```C#
//Initialized without capacity (10 by default)
var stack = new CircularStack<MyType>();

//Initialized with a capacity specified
var stack = new CircularStack<MyType>(200);

//Initialized with a collection specified
//Items will be pushed in the order they are found so Peek() will yield "b"
var items = new List<string> { "a", "b" };    
var stack = new CircularStack<string>(items);

//Initialized with a collection and capacity
var items = new List<string> { "a", "b" };    
var stack = new CircularStack<string>(items, 4);
```

#### Methods:
```C#
//Push an item into the stack
//When full the oldest item in the stack will be removed
public void Push(T item);

//Retrieve and remove last pushed item 
public T Pop();

//Retrieve last pushed Item without removal
public T Peek();
```

#### Properties:
```C#
public int Capacity;
public int Count;
public bool IsFull;
public bool IsEmpty;
```

## Project

CircularStack is licensed under the MIT license, so you can comfortably use it in commercial applications.

## Contributing / Pull Requests

CircularStack uses GitHubFlow for development and integration related work.
Feel free to open up a feature branch and submit a pull request.


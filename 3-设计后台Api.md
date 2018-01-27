---
title: 设计后台Api
tags: [asp.net core, 签到系统 V3.0]
category: 寒假实践
author: yiluomyt
time: 2018-01-24
---

## 设计后台Api

对于签到系统来说，后台的Api主要包括两方面，一对已有数据的查询，二添加新的数据。

据此我们设计以下三个Api：
- `/api/Record` GET 查询数据 参数:begin,end
- `/api/Record/SignIn` POST 新建一条签到记录 参数:user,seat
- `/api/Record/SignOut` POST 记录签退时间 参数:user

## 构建数据模型

本节需要引用的名称空间有：
- System

很显然对于我们这样一个简单的应用来说，仅需要构建一个关于签到记录的模型即可。

示例如下：

(`../Models/Record.cs`)

```c#
public class Record
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateTime SignInTime { get; set; }
    public DateTime? SignOutTime { get; set; }
    public int Seat { get; set; }
}
```

## 创建Api Controller

本节需要引用的名称空间有：
- System
- System.Linq
- System.Collections.Generic
- Microsoft.AspNetCore.Mvc
- PFSign.Models

首先，创建一个Controller 的POCO类，并配置路由信息。

(`../Controllers/RecordController.cs`)

```c#
[Route("/api/[Controller]")]
public class RecordController
{
    //创建一个模拟数据集
    private static List<Record> Records = 
        new List<Record>()
    {
        new Record()
        {
            Id = Guid.NewGuid(),
            UserId = "001",
            Name = "Test1",
            SignInTime = DateTime.Parse("2018-01-24"),
            SignOutTime = DateTime.Parse("2018-01-25"),
            Seat = 1
        },
        new Record()
        {
            Id = Guid.NewGuid(),
            UserId = "002",
            Name = "Test2",
            SignInTime = DateTime.Parse("2018-01-24"),
            Seat = 2
        }
    };

    //...
}
```

下面我们首先构建用于查询的Api。

在默认配置情况下，`/api/Record`即`/api/Record/Index`。

通过C#的Linq语法，我们可以很轻松的完成。

(`../Controllers/RecordController.cs`)

```c#
public object Index
    (DateTime? begin, DateTime? end)
{
    DateTime endTime = end ?? DateTime.UtcNow;
    DateTime beginTime = begin ?? endTime.AddDays(-1);
    return (from r in Records
            where r.SignInTime >= beginTime
            && r.SignInTime <= endTime
            select new
            {
                Name        = r.Name,
                Seat        = r.Seat,
                SignInTime  = r.SignInTime,
                SignOutTime = r.SignOutTime,
            }).ToList();
}
```

接下来，我们来构建签到签退的Api。

形式上，和之前的Api类似，不过要加上`[HttpPost]`属性。

(`../Controllers/RecordController.cs`)

```c#
[HttpPost("[Action]")]
public object SignIn
    (string userId, string name, int seat)
{
    Record record = new Record()
    {
        Id         = Guid.NewGuid(),
        UserId     = userId,
        Name       = name,
        SignInTime = DateTime.UtcNow,
        Seat       = seat
    };
    Records.Add(record);

    return new
    {
        result = true,
        msg    = ""
    };
}

[HttpPost("[Action]")]
public object SignOut(string userId)
{
    Record record = (from r in Records
                     where r.SignOutTime == null
                     && r.UserId == userId
                     select r).FirstOrDefault();
    
    if(record == null)
    {
        return new
        {
            result = false,
            msg    = "未能找到对于记录！"
        };
    }
    
    record.SignOutTime = DateTime.UtcNow;

    return new
    {
        result = true,
        msg    = ""
    };
}
```

至此，后台Api的设计已经完成，接下来我们可以测试一下其基本功能。

## 测试

由于代码已经变更，所以我们需要重新Build。

在命令行中键入以下内容以启动服务器：

```shell
dotnet build
dotnet run
```

以下情形为Build成功，并正常监听5000端口：

![Build&Run](https://blog-1252574286.cossh.myqcloud.com/pfstudio/%E7%AD%BE%E5%88%B0%E7%B3%BB%E7%BB%9FV3.0/3-Build%26Run.PNG)

之后就可以对Api进行调用了，这里我使用的测试工具是Postman。

![Query](https://blog-1252574286.cossh.myqcloud.com/pfstudio/%E7%AD%BE%E5%88%B0%E7%B3%BB%E7%BB%9FV3.0/3-Query.PNG)
# ReduxSharp

C# による単方向データフロー — Redux に着想を得たライブラリ

[![NuGet](https://img.shields.io/nuget/v/ReduxSharp.svg?maxAge=3600)](https://www.nuget.org/packages/ReduxSharp/)


## 概要

JavaScript には Redux というライブラリがあります。
React でシングルページアプリケーションを開発したことがあれば、使ったことがあるかもしれません。
Redux の詳細については、公式ドキュメントを参照してください。

ReduxSharp は Redux を C# に移植したライブラリです。


## インストール

まず、[NuGet をインストール](http://docs.nuget.org/docs/start-here/installing-nuget)します。
次に、パッケージマネージャーコンソールから [ReduxSharp](http://www.nuget.org/packages/ReduxSharp) をインストールします。

```
PM> Install-Package ReduxSharp
```


## クイックスタート

### 状態（State）

アプリの状態を表すクラスを定義します。

```cs
namespace HelloWorld
{
    public class AppState
    {
        public AppState(int count) => Count = count;

        public int Count { get; }
    }
}
```

### アクション（Actions）

アクションを定義します。
アクションは、アプリケーションからストアへデータを送るための情報を格納するペイロードです。

```cs
namespace HelloWorld
{
    public readonly struct Increment
    {
    }

    public readonly struct Decrement
    {
    }
}
```

### リデューサー（Reducers）

リデューサーを定義します。
リデューサーは `IReducer<TState>` インターフェイスを実装する必要があります。
アクションに応じて、現在の状態を次の状態へどのように変換するかを定義します。

```cs
using System;
using ReduxSharp;

namespace HelloWorld
{
    public class AppReducer : IReducer<AppState>
    {
        public AppState Invoke<TAction>(AppState state, TAction action)
        {
            switch (action)
            {
                case Increment _:
                    return new AppState(state.Count + 1);
                case Decrement _:
                    return new AppState(state.Count - 1);
                default:
                    return state;
            }
        }
    }
}
```

### ストア（Store）

`StoreBuilder<TState>` を使ってストアを構築します。
`Store<TState>` は、アクションとリデューサーを結び付けるクラスです。
ストアは次の役割を担います。

- `TState` 型のアプリケーションの状態を保持します。
- `Dispatch<TAction>(TAction action)` を通じて状態を更新できます。
- `Subscribe(IObserver observer)` を通じてリスナーを登録します。`Store<TState>` クラスは `IObservable` を実装しています。

ビルダーに初期状態を渡し、`Build` にリデューサーを渡します。

```c#
using System;
using ReduxSharp;

namespace HelloWorld
{
    class Program : IObserver<AppState>
    {
        static void Main(string[] args)
        {
            Store<AppState> store = new StoreBuilder<AppState>(new AppState(0)).Build(new AppReducer());

            var observer = new Program();
            using (store.Subscribe(observer))
            {
                // 1 を出力します。
                store.Dispatch(new Increment());
                // 2 を出力します。
                store.Dispatch(new Increment());
                // 1 を出力します。
                store.Dispatch(new Decrement());
                // 2 を出力します。
                store.Dispatch(new Increment());
            }
        }

        public void OnNext(AppState value) =>
            Console.WriteLine(value.Count);

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }
}
```


### ミドルウェア（Middleware）

`Use` でファクトリーを実行順に登録し、`Build` を呼び出します。
各ストアは、ディスパッチされるアクションの型ごとにチェーンを一度だけ構築し、キャッシュします。
ファクトリーはデリゲートの構築時に、ストアごと、アクションの型ごとのデータをキャプチャできます。

```csharp
using System;
using ReduxSharp;

public sealed class ConsoleLoggingMiddleware<TState> : IMiddlewareFactory<TState>
{
    public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next)
    {
        string actionName = typeof(TAction).Name;
        return action =>
        {
            Console.WriteLine($"Dispatching {actionName}");
            next(action);
            Console.WriteLine($"Dispatched {actionName}");
        };
    }
}
```

```csharp
Store<AppState> store = new StoreBuilder<AppState>(new AppState(0))
    .Use(new ConsoleLoggingMiddleware<AppState>())
    .Build(new AppReducer());
store.Dispatch(new Increment());
```

登録用の拡張メソッドでは、`IStoreBuilder<TState>` を引数に受け取り、戻り値として返すことができます。
[ログ出力用の拡張メソッド](examples/Benchmark/Examples/LoggingMiddlewareExtensions.cs)を参照してください。

**旧 API からの移行。** `new Store<TState>(reducer, state)` を
`new StoreBuilder<TState>(state).Build(reducer)` に置き換えます。既存の各ミドルウェアについて、
`IMiddleware.Invoke` 内のディスパッチごとの処理を、上記の例のように
`IMiddlewareFactory.Build` が返すデリゲートへ移動します。現在のチェーンの処理を続けるには、
`next.Invoke(action)` を `next(action)` に置き換えます。別の型のアクションを送るには、
`store.Dispatch(otherAction)` を呼び出し、そのアクションのパイプラインを最初のミドルウェアから開始します。
既存のリデューサーと非同期アクションクリエーターは、そのまま利用できます。

## サンプル

- Counter（[ソースコード](https://github.com/tnakamura/ReduxSharp/blob/master/examples/Counter)）
  - WPF アプリ
- RxCounter（[ソースコード](https://github.com/tnakamura/ReduxSharp/blob/master/examples/RxCounter)）
  - WPF アプリ
- TodoList（[ソースコード](https://github.com/tnakamura/ReduxSharp/blob/master/examples/TodoList)）
  - WPF アプリ

## NuGet.org への公開

GitHub Release を公開すると、対象コミットに対して `Publish NuGet package`
ワークフローが実行されます。正式版とプレリリースの両方に対応しています。
タグの作成や push だけではパッケージは公開されません。

### 初回設定

1. `ReduxSharp` パッケージの所有者として NuGet.org にログインし、
   [Trusted Publishing ポリシー](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
   を登録します。Repository Owner は `tnakamura`、Repository は `ReduxSharp`、
   Workflow File は `publish-nuget.yml`（ファイル名のみ）とし、Environment は空欄にします。
   **Glob Patterns and Packages (Separate by a new line)** には `ReduxSharp` を
   1行で入力し、公開対象をこのパッケージ ID に限定します。バージョン番号や `.nupkg` の
   拡張子は付けません。
2. GitHub リポジトリの **Settings > Secrets and variables > Actions** で、
   所有者の NuGet.org ユーザー名をリポジトリ Secret `NUGET_USER` に登録します。
   メールアドレスではありません。長期 API キーの登録は不要です。
3. タグの対象コミットにワークフローが含まれるよう、公開用ワークフローをマージしてから
   リリースタグを作成します。

### バージョンの公開

1. `src/ReduxSharp/ReduxSharp.csproj` の `Version` を更新し、コミットしてマージします。
   `AssemblyVersion` と `FileVersion` は、`Version` からプレリリース接尾辞を除いた値に
   自動設定されます（例: `1.0.0` は `1.0.0.0`）。
2. `Version` と完全に一致するタグ（例: `0.6.0`、`0.6.0-beta.1`）を作成して push します。
   バージョンは先頭に余分なゼロを付けない3つの数値と、必要に応じて SemVer の
   プレリリース接尾辞で構成します。`v` 接頭辞やビルドメタデータは付けません。
3. そのタグを指定して GitHub Release を作成し、リリースノートを準備して
   **Publish release** を選択します。プレリリース版は Release もプレリリースに設定します。
4. **Actions** で実行結果を確認します。評価済みのプロジェクトバージョンとタグの照合、
   ビルド、フォーマット検証、テスト、生成パッケージのバージョン検証を行った後に、
   有効期間の短い公開用キーを取得します。

`.nupkg` と `.snupkg` は Actions の成果物として保存され、NuGet.org に公開されます。
失敗した場合は設定上の問題を修正し、**Re-run failed jobs** で再実行できます。
公開済みのパッケージはスキップし、シンボルは別途送信するため、途中まで公開された場合も
再試行できます。公開済みの NuGet バージョンは置換できないため、コードの修正には
新しいバージョンと Release を使用してください。公開用タグは移動しないでください。

## ライセンス

[MIT](https://opensource.org/licenses/MIT)

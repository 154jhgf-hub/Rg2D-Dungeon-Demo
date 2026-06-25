# Rg2D — 2D 程序化地牢探索 Demo

> 个人独立开发的 Unity 2D Roguelike 原型，涵盖程序化地图生成、战斗、敌人 AI、背包、存档等完整玩法循环。

[![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-000000?logo=unity)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-.NET-512BD4?logo=csharp)](https://docs.microsoft.com/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Personal%20Portfolio-blue)]()

---
## 演示视频
链接：【游戏demo】 https://www.bilibili.com/video/BV1pfjR6uEgW/?share_source=copy_web&vd_source=f8d643174f9d942df40116c7f4d54a62
---

## 项目简介

**Rg2D** 是一款俯视角 2D 地牢探索类 Demo。玩家每次进入关卡时，系统会程序化生成随机地图，在房间与走廊中探索、战斗、拾取道具、升级成长，击败全部敌人后获胜。

本项目为**自学 Unity 期间独立完成**的游戏原型，目标岗位：Unity 客户端开发（初级）。

### 核心玩法

- 程序化生成随机地牢（房间 + 走廊 + 墙体）
- 三种攻击模式：轻击 / 重击 / 远程射击
- 击杀敌人获得经验，升级提升生命与攻击
- 拾取金币、药水、障碍物等场景道具
- 20 格背包：拖拽整理、堆叠、使用消耗品
- 存档 / 读档：保存地图、角色、敌人、掉落物与背包状态
- 消灭所有敌人触发胜利，角色死亡触发失败

---

## 操作说明

| 操作 | 按键 |
|------|------|
| 移动 | `W` `A` `S` `D` |
| 攻击 | `J` |
| 切换攻击类型（轻击 → 重击 → 射击） | `Q` |
| 打开 / 关闭背包 | `H` |
| 打开 / 关闭菜单（暂停） | `M` |

> 移动与攻击使用 **Unity Input System**；背包与菜单暂用 Legacy Input。

---

## 技术栈

| 类别 | 技术 |
|------|------|
| 引擎 | Unity 2022.3 LTS |
| 语言 | C# |
| 渲染 | URP 2D |
| 输入 | Unity Input System |
| 相机 | Cinemachine |
| 地图 | Tilemap + 自定义程序化生成 |
| 数据 | ScriptableObject 配表 |
| 存档 | JsonUtility + File I/O + PlayerPrefs |
| UI | UGUI + TextMeshPro |

---

## 系统设计与技术亮点


### 1. 程序化地牢生成

采用**策略模式**：`AbstractDungeonGenerator` 抽象基类，支持多种生成方案扩展。

```
BSP 空间分割 → 生成房间区域
    ↓
最近邻连通 → 生成 L 形走廊
    ↓
走廊加宽 → 合并地板集合
    ↓
墙体推导 + Tilemap 绘制 → 道具 / 敌人 / 玩家落点
```

- **BSP（二叉空间分割）**：`ProceduralGeneration.BinarySpace` 递归切分区域，保证房间尺寸合法
- **房间连通**：贪心最近邻连接，用 `RoomConnect` 记录房间图结构
- **Random Walk**：可选在房间内做随机游走，生成不规则房间形状
- **墙体生成**：根据地板邻域自动推导墙砖位置

### 2. 场景物件智能放置

`ItemPlaceHelp` 基于 `Graph` 对地板做 8 邻域分析，将格子分类为：

- `OpenSpace`：开阔区域（8 方向均可通行）
- `NearWall`：靠墙区域
- `Wall`：墙体表面

不同道具通过 `ItemData`（ScriptableObject）配置放置类型、尺寸与数量区间，生成时自动选取合法落点，并支持多格占用道具的碰撞检测。

### 3. 敌人 AI（有限状态机）

状态：`Idle → Walk → Attack → Damage → Dead`

| 行为 | 实现 |
|------|------|
| 巡逻 | 随机方向 + BoxCast 障碍检测，撞墙换向 |
| 追击 | 8 方向射线选最优朝向（点积最大且无障碍） |
| 攻击 | 动画事件驱动伤害判定，带攻击 CD |
| 防重叠 | `OverlapCircle` 分离力，避免敌人堆叠 |
| 受击 | 变色反馈 + 协程等待动画结束再切状态 |

### 4. 战斗系统

- `IDamage` 接口统一伤害入口，`Health` 负责生命值与死亡事件
- `Attack` 使用 `Physics2D.OverlapBoxAll` 做近战范围检测
- 远程攻击通过动画事件实例化箭矢，`Shot` 组件处理飞行与碰撞
- `Knockback` 根据攻击来源方向施加击退
- 击杀敌人 → `EnemyManager` 计数 → 全部消灭触发胜利事件

### 5. 背包与拖拽

- 数据层 `InventoryData`（20 格 `Card` 列表）与 UI 层 `InventoryUI` 分离
- `InventoryManager` 通过 `OnInventoryChange` 事件驱动 UI 刷新
- `DragManager` 实现拖拽图标跟随、同 ID 堆叠、异物品换位
- `UseItemManager` 按物品类型分发使用效果（回血 / 加经验 / 加金币）

### 6. 存档系统

`GameData` 将游戏状态拆分为多份 JSON 文件持久化到 `Application.persistentDataPath`：

| 文件 | 内容 |
|------|------|
| `saved_Mapobjects.json` | 地板坐标集合 |
| `saved_objects.json` | 玩家 / 敌人 / Boss 位置、缩放、当前血量 |
| `saved_Itemobjects.json` | 场景固定道具（障碍物、金币等） |
| `saved_Dropobjects.json` | 地面掉落物 |
| `savefile.json` | 背包数据 |
| PlayerPrefs | 金币数量 |

读档时按序重建：地图 → 掉落物 → 场景道具 → 角色与敌人，并重新绑定 Cinemachine 跟随目标。

---

## 架构概览

```
┌─────────────────────────────────────────────────────┐
│                    GameManager                       │
│              （胜负判定 / 全局流程）                  │
└──────────┬──────────────────────────┬───────────────┘
           │                          │
┌──────────▼──────────┐    ┌──────────▼──────────────┐
│  RoomFirstGenerator  │    │      GameData           │
│  （地牢生成 + 落点）  │    │   （存档 / 读档）        │
└──────────┬──────────┘    └─────────────────────────┘
           │
┌──────────▼──────────────────────────────────────────┐
│  ItemPlaceManager  →  玩家 / 敌人 / Boss / 道具生成   │
└─────────────────────────────────────────────────────┘

┌─────────────┐  ┌─────────────┐  ┌──────────────────┐
│ PlayerControl│  │  EnemyAI    │  │ InventoryManager │
│  + Attack   │  │  + Attack   │  │  + DragManager   │
│  + Health   │  │  + Health   │  │  + InventoryUI   │
│  + LevelUp  │  │             │  │  + UseItemManager│
└─────────────┘  └─────────────┘  └──────────────────┘
```

**使用的设计模式：**

- 单例（`GameManager`、`InventoryManager`、`EnemyManager` 等）
- 观察者（`event Action` 驱动 UI 与流程）
- 策略（多种地牢生成器继承同一抽象基类）
- 接口（`IDamage` 统一伤害逻辑）
- 数据驱动（`ItemData`、`LevelDataSO`、`RandomWalkSO`）

---

## 项目结构

```
Assets/
├── Actions/              # Input System 输入配置
├── Editor/               # 地牢生成编辑器扩展按钮
├── Resources/            # 美术与音频资源
├── Scenes/
│   └── SampleScene.unity
└── Scripts/
    ├── AbstractDungeonGenerator.cs   # 地牢生成抽象基类
    ├── RoomFirstGenerator.cs         # 房间优先生成器
    ├── CorridorFirstDungeonGenerator.cs
    ├── ProceduralGeneration.cs       # BSP / RandomWalk 算法
    ├── Graph.cs                      # 地图邻域图
    ├── Combat/                       # 战斗、生命、升级、射击
    ├── Enemy/                        # 敌人 AI 与管理
    ├── Inventory/                    # 背包数据、UI、拖拽
    ├── Item/                         # 金币、掉落物
    ├── Place/                        # 场景物件放置
    ├── Save/                         # 存档系统
    ├── Player/                       # 玩家控制
    ├── UI/                           # 菜单与 HUD
    ├── Audio/                        # 音频管理
    └── Data/                         # ScriptableObject 配表
```
## 如何运行

### 环境要求

- Unity **2022.3.62f1** 或同系列 LTS 版本
- Windows / macOS 编辑器均可

### 步骤

1. Clone 本仓库
2. 用 Unity Hub 打开项目根目录
3. 打开场景 `Assets/Scenes/SampleScene.unity`
4. 点击 Play 运行

### 编辑器快捷操作
选中场景中的地牢生成器组件，Inspector 面板提供 **Generate Dungeon** 按钮，可在编辑器中手动重新生成地图。
---
## License

本项目仅用于个人求职作品集展示，美术资源版权归原作者所有，请勿用于商业用途。

# MC Engine

一个使用 **Unity** 开发的 2D 沙盒冒险游戏项目。游戏以探索、采集、建造、战斗和成长为核心循环，并计划通过动态生态系统形成区别于传统沙盒游戏的特色体验。

## 为什么选择 Unity

Unity 很适合本项目，主要原因如下：

- **成熟的 2D 工具链**：Tilemap、2D Physics、Sprite Atlas、2D Animation 和 URP 2D Renderer 可以覆盖地图、角色、光照及碰撞等核心需求。
- **适合程序化世界**：C# 便于实现区块生成、噪声地形、生态群系、存档和对象池等系统，同时拥有良好的开发效率与可维护性。
- **性能分析工具完整**：Profiler、Frame Debugger 和 Memory Profiler 有助于定位大地图、动态光照及大量实体带来的性能问题。
- **跨平台支持成熟**：项目可以优先开发 Windows 版本，之后再扩展至 macOS、Linux 和主机平台。
- **生态系统丰富**：Asset Store、官方包和大量社区资料能够减少编辑器工具、输入系统、镜头及 UI 等通用功能的开发成本。
- **便于团队协作**：组件化架构、Prefab、ScriptableObject 和自定义编辑器适合多人并行制作内容。

相比自行开发引擎，Unity 能让项目把更多时间投入玩法和内容；相比一些轻量引擎，Unity 在大型项目工具、性能诊断、平台发布和第三方资源方面更加成熟。代价是引擎本身较重，因此项目会固定 Unity 与依赖包版本，并谨慎引入第三方插件。

## 推荐技术方案

- 当前稳定的 **Unity 6 LTS**
- Tilemap 构建可破坏的方块世界
- New Input System 统一键鼠和手柄输入
- ScriptableObject 管理物品、配方、敌人和生态配置
- 分区存储与按需加载支持大型程序化地图

## 当前可玩内容

仓库已经包含一个不依赖美术资源的可玩原型：

- 按种子程序化生成地表、洞穴、矿层和树木
- 基于 Tilemap 的可碰撞方块世界
- 角色移动、跳跃与镜头跟随
- 射程限制、方块耐久和按住连续挖掘
- 数字键及滚轮快捷栏选择
- 木板、石砖配方和木镐、石镐工具成长
- 可堆叠的方块背包
- JSON 世界存档、旧存档迁移以及自动读取
- 合并 Tilemap 碰撞和批量方块刷新
- 可由 Unity Inspector 配置的角色与方块素材目录
- EditMode 测试覆盖世界、背包、合成、成长和存档核心逻辑

## 运行项目

1. 使用 Unity Hub 安装 Unity `6000.0.60f1`，或使用兼容的 Unity 6 LTS 补丁版本。
2. 在 Unity Hub 中选择 **Add project from disk**，打开仓库根目录。
3. 等待 Package Manager 完成依赖恢复和脚本编译。
4. 如果 Unity 询问是否启用新版 Input System 后端，选择启用并让编辑器重启。
5. 首次导入时，项目会自动创建 `Assets/Scenes/Main.unity` 并加入 Build Settings。
6. 打开 `Main` 场景并进入 Play Mode。

项目当前使用运行时代码生成的纯色方块和角色，因此不需要先导入图片资源。

### 操作方式

| 操作 | 按键 |
| --- | --- |
| 左右移动 | `A` / `D` 或方向键 |
| 跳跃 | `Space` |
| 持续挖掘方块 | 按住鼠标左键 |
| 放置方块 | 鼠标右键 |
| 选择泥土、石头、木头、木板、石砖 | `1` 至 `5` 或鼠标滚轮 |
| 制作 4 个木板 | `C`，消耗 1 个木头 |
| 制作 2 个石砖 | `B`，消耗 2 个石头 |
| 制作木镐 | `R`，消耗 8 个木头 |
| 制作石镐 | `T`，需要木镐并消耗 15 个石头 |
| 保存世界 | `F5` |
| 创建新世界 | `F9` |

存档位于 Unity 的 `Application.persistentDataPath` 目录，退出游戏时也会自动保存。

## 如何与 Unity 结合

项目分为两层：`Runtime` 中的 C# 负责游戏规则，Unity 负责场景生命周期、输入、物理、Tilemap、渲染和资源配置。当前 `GameBootstrap` 会在进入 Play Mode 后自动组装世界、玩家、相机和 HUD，因此不需要手工拖放一整套 Prefab 就能运行。

### 1. 第一次导入

1. 在 Unity Hub 中添加仓库根目录，而不是只添加 `Assets` 目录。
2. 等待右下角的脚本编译和 Package Manager 依赖恢复结束。
3. 同意启用 New Input System；编辑器要求重启时保存并重启。
4. `InitialSceneCreator` 会创建空的 `Assets/Scenes/Main.unity`，运行时对象则由 `GameBootstrap` 创建。
5. 打开 `Main` 场景，点击顶部 Play 按钮即可测试。

如果 Unity 没有自动创建场景，可手动创建一个空场景并保存为 `Assets/Scenes/Main.unity`。场景中不需要放置对象。

### 2. 替换占位美术

不配置素材时，项目会继续使用代码生成的纯色图块。接入正式像素素材时：

1. 创建 `Assets/Resources` 目录。
2. 在 Project 窗口中右键，选择 **Create > MC Engine > Game Content Catalog**。
3. 将资产保存为 `Assets/Resources/GameContentCatalog.asset`；文件名和位置必须保持一致。
4. 导入 PNG 后，在 Inspector 中设置 **Texture Type = Sprite (2D and UI)**、**Filter Mode = Point**、**Compression = None**。
5. 方块图应使用统一尺寸，并把 **Pixels Per Unit** 设置为单个方块的像素宽度，例如 16×16 图块使用 16 PPU。
6. 在 `GameContentCatalog` 中设置 Player Sprite，并为 Grass、Dirt、Stone、Wood、Bedrock、Plank 和 StoneBrick 分别添加图块。
7. 再次进入 Play Mode；`Resources.Load` 会读取目录资产并替换占位图。

玩家图片会自动缩放到约 `0.8 × 1.8` 个世界单位，方块图片则应保持 `1 × 1` 个世界单位。

### 3. 在 Unity 中调试与测试

- 使用 **Window > Analysis > Profiler** 查看脚本、物理和渲染开销。
- 在 **Window > General > Test Runner > EditMode** 中选择 **Run All** 运行核心逻辑测试。
- Play Mode 中按 `F5` 后，可在 Console 状态和 `Application.persistentDataPath` 对应目录检查存档。
- Tilemap 使用 Chunk 渲染；`TilemapCollider2D` 通过 `CompositeCollider2D` 合并碰撞边界，适合当前小型世界。

### 4. 构建可执行文件

1. 打开 **File > Build Profiles**。
2. 选择 Windows、macOS 或 Linux 平台并安装缺少的平台模块。
3. 确认 `Assets/Scenes/Main.unity` 已添加且启用。
4. 设置 Product Name、版本号和图标后选择 **Build**。

首次发布前应在目标平台重新创建世界并验证输入、存档路径、分辨率和退出自动保存。

## 代码结构

```text
Assets/MC-Engine/
├── Editor/             # 首次导入时创建启动场景
├── Runtime/
│   ├── Bootstrap/      # 自动组装可玩原型
│   ├── Configuration/  # Unity Inspector 内容目录
│   ├── Gameplay/       # 游戏会话和世界交互
│   ├── Inventory/      # 方块背包
│   ├── Persistence/    # JSON 存档
│   ├── Player/         # 移动和镜头
│   ├── Presentation/   # Tilemap 与 HUD
│   └── World/          # 世界数据和地形生成
└── Tests/EditMode/     # 核心逻辑测试
```

## 首个可玩版本

第一阶段制作一个约 20 至 30 分钟的单人垂直切片：

1. 生成一个包含地表与洞穴的小型世界。
2. 实现移动、挖掘、放置、背包和基础合成。
3. 加入三种普通敌人、一名 Boss 和十余件装备。
4. 建造符合条件的房屋后允许一名 NPC 入住。
5. 加入一个会因玩家行为发生变化的生态区域。
6. 支持创建世界、退出保存和继续游戏。

多人联机、复杂液体、电路系统和大规模生态模拟将在核心循环验证完成后再考虑。

## 后续开发路线

当前版本用于验证最基础的“探索、挖掘、放置”循环，还不是完整成品。后续按以下顺序扩展：

1. 通用物品定义、掉落物和可视化合成界面。
2. 角色生命、敌人 AI 和战斗。
3. 区块化世界渲染、后台生成和增量存档。
4. 房屋判定、NPC、交易和任务。
5. 生态变化事件、Boss 及内容阶段推进。
6. 正式美术、音频、设置、无障碍和发布流程。

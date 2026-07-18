# DeusExMachina · 下落式音游原型（Unity / PC）

一个从零开始、**按 Play 即可运行**的下落式音游（4K）最小原型。核心目标是先把音游最难的部分——
**音画/判定同步**——用 Unity 正确的方式（`AudioSettings.dspTime` + `PlayScheduled`）跑通。

## 文件结构

```
Assets/RhythmGame/
├── Scripts/
│   ├── Conductor.cs          # 计时核心：以 dspTime 为时间基准，含偏移校准
│   ├── ChartModels.cs        # 谱面数据模型 + JSON 读写
│   ├── Lane.cs               # 单条轨道（背景 + 按下闪光 + 按键），由 Playfield 持有
│   ├── Playfield.cs          # 整块判定区：N 条轨道 + 判定线，打包成一个可拖入场景的预制体
│   └── RhythmGameManager.cs  # 主控：读 Playfield 布局 / 判定 / 计分 / 输入 / HUD / Demo
├── Editor/
│   └── RhythmPrefabBuilder.cs # 一键生成 Playfield / Note / HoldBody / 判定弹出 预制体的编辑器菜单
├── Charts/
│   └── sample.json           # 示例谱面
└── Generated/                # 运行编辑器菜单后生成的预制体与白色精灵
```

> 判定区（轨道+判定线）做成一个 **Playfield 预制体**，你把它拖进场景后自由编辑；管理器运行时从中读取布局。
> 若场景里没有 Playfield，管理器会用程序化精灵搭一个兜底，零资源也能直接跑。

## Playfield 工作流（核心）

1. 选中挂了 `RhythmGameManager` 的物体，点菜单 **`Tools > Rhythm > Generate Default Prefabs`**。
   会在 `Assets/RhythmGame/Generated/` 生成 `Playfield.prefab`（4 轨 + 判定线 + 可见上界标记）以及 `Note` / `HoldBody`，
   后两者自动赋给管理器。
2. **把 `Playfield.prefab` 拖进场景**（管理器会自动在场景中找到它，无需手动赋值，也可手动拖到 `Playfield` 字段）。
3. 双击 / 进入 `Playfield.prefab` 编辑：
   - 增删轨道（记得把每条 `Lane` 加进根节点 `Playfield` 组件的 `lanes` 列表，顺序即 0..N 号轨道）；
   - 移动轨道改间距、缩放背景改轨宽（音符宽度会自动跟随轨宽）；
   - 改判定线的 Y、移动 `SpawnTop` 标记改音符出现高度；
   - 每条 `Lane` 上设它自己的触发 **按键**、闪光颜色与不透明度，添加装饰子物体。

**职责划分（重要）**：

- **判定区布局 + 外观 = Playfield 预制体**：轨道数量/位置/宽度、判定线高度、可见上界、按键、颜色。
- **音符几何 = 管理器**：音符按轨道中心 X 落下，宽度跟随轨宽，长按条随时长动态拉伸。
  因此 `Note` / `HoldBody` 预制体建议保持「根节点单个 SpriteRenderer」（外观可改，几何被管理器接管）。

## 快速开始

1. 用 Unity Hub 新建一个工程（**2D 或 3D 模板都行**），把本仓库的 `Assets` 文件夹合并进工程的 `Assets`。
2. 新建空场景 → 创建一个空物体（`GameObject > Create Empty`）→ 给它挂上 **`RhythmGameManager`** 脚本。
3. （可选但推荐）按上面的 Playfield 工作流生成并拖入 `Playfield.prefab`。
4. 直接按 **Play**。
   - 不指定歌曲时，会自动生成一段 **150 BPM 节拍器音轨 + 匹配谱面**，音符落到判定线时正好有"哒"声，方便你校准手感。
   - 没拖 Playfield 也能跑：管理器会按「程序化兜底参数」自动搭一个 4 轨判定区。

### 用自己的歌曲和谱面

在 `RhythmGameManager` 的 Inspector 上：

- **Song**：拖入你的音频（建议导入设置 Load Type 用 `Decompress On Load`，减少播放抖动）。
- **Chart Json**：拖入 `Charts/sample.json`（或你自己的谱面 `.json`，Unity 会自动当作 TextAsset）。

## 操作

| 按键 | 作用 |
|------|------|
| `D` `F` `J` `K` | 4 条轨道（每条轨道的按键在 `Playfield` 里的对应 `Lane` 上设置） |
| `F1` / `F2` | 音频偏移 −/+ 5ms（补偿声卡延迟，自动存档） |
| `F3` / `F4` | 输入偏移 −/+ 5ms（补偿手感/外设延迟） |
| `F5` | 偏移归零 |
| `R` | 重开 |

> 校准方法：先调 `F1/F2` 让"音符到线"和"听到的拍点"对齐；再调 `F3/F4` 让你**自认为压准**时拿到最多 PERFECT。

## 击中提示（Perfect/Great/Good/Miss 弹出）

击中（或 Miss）音符时，会在**被击中轨道的判定线处**弹出一个提示，四种判定各用一个独立 prefab。
相关脚本在 `Assets/Scripts/`：

- `JudgementPopupSpawner.cs`：挂在场景任意物体上，提供四个 prefab 槽（`Perfect/Great/Good/Miss`，留空的档位不弹出）。
  订阅管理器的 `JudgedAt` 事件，按判定档位在击中位置实例化对应 prefab。
  - **贴合 Playfield 的关键**：把 `spawnParent` 设为 **Playfield 下的一个子物体**（例如它里面的 World Space Canvas，或一个空物体）。
    弹出物会成为它的子物体并在其**局部空间**里定位，于是自动继承整条父链（Playfield→Canvas）的旋转/缩放/位移——无需再手动对齐旋转。
  - `spawnParent` 留空时直接放到场景根，不跟随 Playfield。
  - `reuseWhenSameJudgment`（默认开）：本次判定与上一个相同且上一个提示还在时，复用它并重播动画而不是新建，
    避免固定位置下相同判定越堆越多（需要 prefab 上有 `JudgementPopup` 才能重播）。
  - `clearPreviousOnNew`（默认开）：新建提示前先把上一个立即销毁，保证“新提示出现时旧的已完全消失”，不会两个并存淡出。
    与上一项配合即“同一时刻只保留一个提示”。
- `JudgementPopup.cs`（可选）：挂在弹出 prefab 上的通用动画——入场放大回弹 + 上浮 + 末段淡出 + 到时自销毁，
  全程在**局部空间**做动画，所以在 Playfield 子物体下会一起贴合。对子物体里的 `TMP_Text` 与 `SpriteRenderer` 都生效。
  想用自己的 Animator/粒子就不挂它，由 Spawner 的 `fallbackLifetime` 兜底清理。

**一键起步**：菜单 **`Tools > Rhythm > Generate Judgement Popup Prefabs`** 会生成一个共享基底 `Popup_Base.prefab`，
并把 `Popup_Perfect/Great/Good/Miss` 做成它的**预制体变体（Prefab Variant）**——四个变体只覆盖各自的**文字与颜色**，
其余（字号/对齐/排序/`JudgementPopup` 动画参数等）都由 `Popup_Base` 统一控制：**改 Base，四个一起变**。
生成后会自动指给场景里的 `JudgementPopupSpawner`，`spawnParent` 默认指向 Playfield。之后把文字换成你自己的图案/特效即可。

> 想手动建：在 Project 里右键 `Popup_Base` → `Create > Prefab Variant`，重命名后只改文字/颜色即可；
> 个别档位要不一样（比如某个更大）就在该变体上单独覆盖那条属性，不影响其它变体。

> 管理器对外事件：`JudgedAt`（带判定档位、轨道编号、判定线世界坐标）。另有 `Judged`（仅档位，计数 UI 用）与 `ComboChanged`。

## 谱面 JSON 格式

```json
{
  "title": "曲名",
  "artist": "作者",
  "laneCount": 4,
  "bpm": 120,
  "offset": 0,
  "notes": [
    { "lane": 0, "time": 2.0, "duration": 0 },
    { "lane": 1, "time": 5.0, "duration": 1.0 }
  ]
}
```

- `lane`：轨道下标，从 0 开始。
- `time`：击打时刻（秒，相对歌曲起点）。
- `duration`：`0` = 单点 Tap；`>0` = 长按 Hold（秒）。
- `offset`：音频起点到第一拍的空拍（秒），用于整体对齐。

## 重要：两个常见环境坑

1. **输入系统**：本原型用旧版 `Input`。若工程报 `InvalidOperationException: ... Input System`，去
   `Project Settings > Player > Active Input Handling` 改为 **Both** 或 **Input Manager (Old)**。
2. **音频延迟**：`Project Settings > Audio > DSP Buffer Size` 设为 **Best latency**，可显著降低判定延迟。

## 已实现 vs 后续可扩展

- 已实现：dspTime 精确同步、Tap/Hold 判定、Perfect/Great/Good/Miss 判定窗口、连击/分数/准度、
  实时偏移校准与存档、自动 Demo 谱面、轨道按键反馈、击中位置的判定弹出提示（四种各一 prefab）。
- 建议后续：暂停/恢复（恢复时需重新对齐音频）、变 BPM / 变速(SV)、谱面编辑器、对象池（超大谱面）、
  成绩结算页、打击特效与音效、设置菜单与选曲界面。
```

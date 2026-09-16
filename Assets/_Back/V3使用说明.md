# 灰蓝水墨云雾背景 / Ink Cloud Background

## V3 左下轨道保护版

保留灰蓝水墨与固定网点。速度 Flow Speed 由 V2 的 0.3 调为 0.6，运动快两倍。Coverage 从 1 降到 0.85，减少云雾覆盖。取消中央留白，固定保护屏幕左侧 70% 宽、底部 28% 高的轨道区，这个区域始终不含墨色。外侧 12% 范围柔和过渡。White Space 控制保护区高度；保护区不随噪声或动画移动。实际轨道布局变化时需对应调整。运动没有绑定音乐节拍。

## 导入与预览

1. 在使用 Universal Render Pipeline (URP) 的 Unity 工程中，导入 InkCloudBackground.unitypackage。
2. 打开 Assets/InkCloudBackground/InkCloudDemo.unity，点击 Play 查看持续流动的背景。
3. 选中层级中的 Ink Cloud Background，在 Inspector 的组件中调整参数。
4. 在自己的场景中，把 InkCloudBackground.prefab 拖入层级，Target Camera 指向游戏摄像机；留空时自动寻找标记为 MainCamera 的摄像机。

## 美术参数

| 参数 | 含义 |
| --- | --- |
| Paper Color | 留白颜色，默认近白 |
| Ink Color | 灰蓝色墨色 |
| Density | 浓淡；0 为无云雾，默认 1 |
| Coverage | 覆盖范围；默认 1，降低可增加留白 |
| Watercolor Variation | 内部深浅变化强度 |
| Flow Speed | 流动速度；默认 0.6，0 为静止 |
| Dots Across Width | 横向网点数量；数值越大，圆点越细 |
| Halftone Strength | 边缘网点感；0 为柔和渐变，1 为明显网点 |
| Seed | 改变云纹初始形态 |
| Animate In Editor | 非运行状态下是否预览动画 |
| Preview Time | 关闭编辑器动画后指定静帧时间 |

像在 AE 中调效果控件一样，直接调背景对象的组件。组件会覆盖材质同名参数，因此通常不需要编辑材质。运行模式中调好的数值会在退出 Play 后还原；可在退出前复制组件，再在退出后粘贴组件数值，或在非运行状态调整。

## 同事接入说明

- 资源不含人物、UI、音频、外部图片和插件。材质用实时多层噪声生成云雾，时间连续推进，不播放有首尾的动画片段。
- 当前为 URP 专用 Shader，演示工程版本为 Unity 6.6 / URP 17.6。Built-in 和 HDRP 需要移植 Shader，不能直接使用。其他版本和目标设备需在实际工程验证。
- 背景是随摄像机铺满画面的不受光平面，渲染队列 2900（天空盒之后、常规透明物体之前），不写深度。Distance 默认 50，应放在所有需要显示的世界物体后面，并位于摄像机远裁剪面以内。相机 Culling Mask 必须包含背景所在层。自定义透明物体队列应大于 2900。
- 推荐将预制体放在场景根层级，避免父物体缩放。支持普通透视和正交相机；非对称投影、XR、多相机叠加需要同事按项目适配。
- 使用单个目标摄像机；若多摄像机分别需要背景，应创建各自实例并用 Layer/Culling Mask 隔离。
- 默认包含不透明白底，不是透明烟雾叠加层。前景 UI 可正常绘制在上面。全屏后处理可能影响墨色，演示场景不启用后处理。
- V3 云团自由游走与旋转。Coverage 改大小（默认 0.85），Cloud Travel 改游走幅度，White Space 改左下轨道保护区高度（默认屏幕高度 28%）。中央允许云雾经过。
- 网点固定在画面坐标上，动画只改变圆点覆盖。改变分辨率或窗口比例会改变像素间距。小尺寸画面建议减少 Dots Across Width，防止网点过细。
- 当前约为一个全屏绘制、每像素多层程序噪声；未针对移动端或 VR 做性能预算。应在目标设备测量 GPU 用时，必要时减少噪声层数。
- 游戏暂停（timeScale 为 0）时云雾暂停。重新进入 Play 会从初始形态开始；持续播放过程中不主动重置时间。长时间运行仍应做项目级稳定性测试。

## 文件

- InkCloud.shader：实时水墨与网点材质逻辑。
- InkCloudBackground.cs：摄像机适配、动画时间与美术参数。
- InkCloud.mat：材质。
- InkCloudBackground.prefab：可拖入场景的背景对象。
- InkCloudDemo.unity：独立预览场景。
- Editor/InkCloudSetup.cs：Tools > Ink Cloud 菜单，用于创建演示场景与导出资源包；不会包含在游戏运行代码中。

这是 V3 左下轨道保护版。导入后打开 InkCloudDemo 场景点击 Play 预览。

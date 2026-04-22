# 📦 Tổng Quan Package: Assets/Gif

Thư mục `Assets/Gif` chứa **2 package** chính:

| Package | Tác giả | Mục đích |
|---|---|---|
| `Ruccho/FangAutoTile` | Ruccho | Tool tạo Auto Tile trong Unity Editor |
| `Super_Retro_Collection` | — | Asset bộ sprite RPG retro + script mẫu |

---

## 📂 1. Ruccho / FangAutoTile

Auto Tile tool cho Unity Tilemap. Tự động chọn đúng sprite dựa trên các ô lân cận (8 hướng), hỗ trợ animation và padding.

### Cấu trúc thư mục

```
Ruccho/FangAutoTile/
├── Scripts/
│   ├── Runtime/
│   │   ├── FangAutoTile.cs          ← Runtime: TileBase tùy chỉnh
│   │   ├── FangAutoTilePacker.cs    ← Runtime: ScriptableObject packer
│   │   ├── AssemblyInfo.cs          ← Thông tin assembly
│   │   └── Legacy/
│   │       ├── FangAutoTile.cs      ← Phiên bản cũ (Legacy)
│   │       └── FangAutoTilePattern.cs ← Pattern cũ (Legacy)
│   └── Editor/
│       ├── FangAutoTileEditor.cs    ← Editor: Custom Inspector cho tile
│       └── FangAutoTilePackerEditor.cs ← Editor: Custom Inspector cho packer
└── Sample/
    ├── OnMapPlayerController.cs     ← Script mẫu di chuyển nhân vật
    ├── sample.unity                 ← Scene demo
    ├── PixelSnapped.mat             ← Material pixel-perfect
    ├── MapChip_pipo/                ← Texture mẫu tileset
    ├── Tiles/                       ← Tile assets mẫu
    └── Sample/                      ← Assets mẫu bổ sung
```

---

### 📄 FangAutoTile.cs (Runtime)

**Namespace:** `Ruccho.Fang`  
**Kế thừa:** `TileBase`  
**Menu:** `Assets > Create > Fang > Tile`

#### Chức năng
Là tile thông minh tự động nhận diện 8 ô xung quanh và chọn sprite phù hợp (auto-tiling kiểu RPG/JRPG).

#### Các thuộc tính chính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `enablePadding` | `bool` | Thêm pixel viền để tránh artifact |
| `oneTilePerUnit` | `bool` | Tile chiếm đúng 1 unit |
| `pixelsPerUnit` | `int` | Pixel trên mỗi unit (mặc định 16) |
| `wrapMode` | `TextureWrapMode` | Wrap mode của texture |
| `filterMode` | `FilterMode` | Filter mode (Point = pixel art) |
| `mainChannel` | `Texture2D` | Texture chính của tile |
| `subChannels` | `Texture2D[]` | Các lớp texture bổ sung |
| `animationMinSpeed` | `float` | Tốc độ animation tối thiểu |
| `animationMaxSpeed` | `float` | Tốc độ animation tối đa |
| `animationStartTime` | `float` | Thời điểm bắt đầu animation |
| `colliderType` | `Tile.ColliderType` | Kiểu collider (None/Sprite/Grid) |
| `connectableTiles` | `TileBase[]` | Tile nào được coi là "nối liền" |

#### Các phương thức chính

| Phương thức | Mô tả |
|---|---|
| `RefreshTile()` | Làm mới tile và 8 ô xung quanh khi thay đổi |
| `GetTileData()` | Tính toán bitmask 8-bit từ ô lân cận, trả về sprite đúng |
| `GetTileAnimationData()` | Cung cấp dữ liệu animation nếu tile có nhiều frame |
| `TileValue()` | Kiểm tra xem ô có phải tile cùng loại hay không |
| `GetAllSprites()` | Trả về tất cả sprites trong tile (dùng bởi packer) |

#### Logic auto-tiling
Dùng bitmask 8-bit (8 hướng xung quanh → 256 trạng thái). Mỗi góc tile được phân loại thành 5 kiểu:
- `0` = Góc trống
- `1` = Chỉ có cạnh dọc
- `2` = Chỉ có cạnh ngang
- `3` = Có cả hai cạnh, không có góc
- `4` = Tất cả nối liền (full)

---

### 📄 FangAutoTilePacker.cs (Runtime)

**Namespace:** `Ruccho.Fang`  
**Kế thừa:** `ScriptableObject`  
**Menu:** `Assets > Create > Fang > Packer`

#### Chức năng
Container ScriptableObject cho phép ghép nhiều `FangAutoTile` vào **1 texture atlas duy nhất**, giúp tối ưu draw call và bộ nhớ GPU.

#### Các thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `targets` | `FangAutoTile[]` | Danh sách tile cần đóng gói |
| `enablePadding` | `bool` | Padding cho atlas |
| `wrapMode` | `TextureWrapMode` | Wrap mode của atlas |
| `filterMode` | `FilterMode` | Filter mode của atlas |
| `compiledChannels` | `Texture2D[]` | Texture atlas đã tạo (ẩn) |

---

### 📄 FangAutoTileEditor.cs (Editor)

**Namespace:** `Ruccho.Fang`  
**Kế thừa:** `Editor`  
**CustomEditor cho:** `FangAutoTile`

#### Chức năng
Custom Inspector trong Unity Editor cho `FangAutoTile`. Cung cấp giao diện trực quan để cấu hình và generate tile.

#### Các tính năng giao diện

| Tính năng | Mô tả |
|---|---|
| **Tile Settings Foldout** | Cấu hình animation speed, collider, connectable tiles |
| **Tile Generation Settings Foldout** | Cấu hình texture source, padding, PPU |
| **Nút "Generate!"** | Chạy toàn bộ quy trình tạo sprite từ texture |
| **Nút "Clear all generated contents"** | Xóa toàn bộ sprites/textures đã generate |
| **Info Box** | Hiển thị số combination, frame, tile size, texture size |
| **Validity Check** | Kiểm tra texture đầu vào hợp lệ trước khi generate |

#### Các phương thức nội bộ quan trọng

| Phương thức | Mô tả |
|---|---|
| `GenerateCombination()` | Tính tất cả 256 tổ hợp láng giềng → nhóm thành hash ID |
| `GenerateTexture()` | Vẽ các segment tile vào texture atlas 2D |
| `GetSegments()` | Lấy tất cả TileCombinationSegment (vùng cắt trong texture gốc) |
| `GetSuitableTextureSize()` | Tìm texture size tối thiểu dạng power-of-2 để chứa tất cả segments |
| `GenerateTilesForTexture()` | Vẽ segments vào texture đích + tạo Sprite asset |
| `CheckValidity()` | Validate texture đầu vào (kích thước, format) |
| `RenderStaticPreview()` | Hiển thị preview trong Project window |

---

### 📄 FangAutoTilePackerEditor.cs (Editor)

**Namespace:** `Ruccho.Fang`  
**Kế thừa:** `Editor`  
**CustomEditor cho:** `FangAutoTilePacker`

#### Chức năng
Custom Inspector cho `FangAutoTilePacker`. Cho phép generate atlas chứa nhiều tile cùng lúc.

#### Luồng hoạt động `Generate()`

1. Thu thập tất cả `FangAutoTileEditor` từ `targets`
2. Kiểm tra tính hợp lệ của từng tile
3. Kiểm tra số sub-channel phải bằng nhau
4. Gọi `GenerateCombination()` cho từng tile
5. Tính `texSize` phù hợp cho tất cả segments
6. Vẽ main channel → sub channel vào atlas
7. Lưu assets và import

---

### 📄 OnMapPlayerController.cs (Sample)

**Namespace:** `Ruccho`  
**Kế thừa:** `MonoBehaviour`  
**RequireComponent:** `Rigidbody2D`

#### Chức năng
Script mẫu điều khiển nhân vật theo **grid-based movement** (di chuyển ô vuông) — phù hợp cho game RPG top-down kiểu classic.

#### Các thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `speedPerFrame` | `float` | Tốc độ di chuyển giữa các ô |

#### Logic di chuyển

| Phương thức | Mô tả |
|---|---|
| `Start()` | Canh chỉnh vị trí về trung tâm ô (grid snap) |
| `Update()` | Lerp đến `targetPos`, gọi `UpdateTarget()` khi đến nơi |
| `UpdateTarget()` | Đọc input Arrow Keys, dùng Raycast kiểm tra tường |
| `OnDrawGizmos()` | Vẽ debug line và sphere raycast trong Scene view |

**Đặc điểm:** Di chuyển từng ô một, có raycast kiểm tra vật cản theo X, Y, và diagonal.

---

## 📂 2. Super_Retro_Collection

Bộ asset phong cách RPG retro với sprites nhân vật, tileset, và scripts mẫu.

### Cấu trúc thư mục

```
Super_Retro_Collection/
├── Scripts/
│   ├── CharacterAppearance.cs     ← Quản lý animation và sprite sheet
│   └── PlayerMovement.cs          ← Di chuyển nhân vật cơ bản
├── Resources/
│   ├── Characters/                ← Sprite sheets nhân vật
│   ├── ARPG/                      ← Sprites ARPG
│   ├── Animations/                ← Clips animation
│   ├── Backgrounds/               ← Backgrounds và tiles
│   ├── Battlers/                  ← Sprite chiến đấu
│   ├── Environments/              ← Tile môi trường
│   ├── Hero/                      ← Sprite nhân vật chính
│   ├── Prefabs/                   ← Prefab cơ bản
│   ├── Prefabs_with_behavior/     ← Prefab có script
│   └── color_palette.png          ← Bảng màu chính thức
└── Samples/
    ├── behavior_sample.unity      ← Scene demo behavior
    ├── farm_sample.unity          ← Scene demo nông trang
    ├── forest_sample.unity        ← Scene demo rừng
    ├── gigantic_tree_sample.unity ← Scene demo cây khổng lồ
    ├── indoor_sample.unity        ← Scene demo trong nhà
    ├── marketplace_sample.unity   ← Scene demo chợ
    ├── prefabs_sample.unity       ← Scene demo prefabs
    ├── tower_sample.unity         ← Scene demo tháp
    └── village_sample.unity       ← Scene demo làng
```

---

### 📄 CharacterAppearance.cs

**Kế thừa:** `MonoBehaviour`  
**Pattern:** Singleton (`public static instance`)

#### Chức năng
Quản lý **animation** và **sprite sheet swapping** cho nhân vật. Tự động load sprite sheet từ `Resources/Characters/` và cập nhật hướng nhìn/tốc độ animation.

#### Các thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `tf` | `Transform` | Transform của nhân vật |
| `movement` | `Vector2` | Vector di chuyển hiện tại |
| `animator` | `Animator` | Animator component |
| `spriteRenderer` | `SpriteRenderer` | SpriteRenderer component |
| `SpriteSheetName` | `string` | Tên sprite sheet (thay đổi được trong Inspector) |

#### Các phương thức

| Phương thức | Mô tả |
|---|---|
| `Awake()` | Khởi tạo singleton, load sprite sheet mặc định |
| `FixedUpdate()` | Tính delta position → gọi `animationUpdate()` |
| `LateUpdate()` | Kiểm tra sprite sheet thay đổi, swap sprite |
| `animationUpdate()` | Set Animator params: `speed`, `orientation` (0/2/4/6) |
| `LoadSpriteSheet()` | Load sprite sheet từ `Resources/Characters/{name}/spritesheet` |

#### Animation Parameters

| Parameter | Kiểu | Giá trị |
|---|---|---|
| `speed` | `float` | Tổng |Δx| + |Δy| (0 = đứng yên) |
| `orientation` | `int` | 0=lên, 2=trái, 4=xuống, 6=phải |

---

### 📄 PlayerMovement.cs

**Kế thừa:** `MonoBehaviour`  
**Pattern:** Singleton (`public static instance`)

#### Chức năng
Script di chuyển nhân vật đơn giản dùng **Rigidbody2D** và **Input Axis** (WASD/Arrow keys).

#### Các thuộc tính

| Thuộc tính | Kiểu | Mô tả |
|---|---|---|
| `moveSpeed` | `float` | Tốc độ di chuyển (mặc định: 5) |
| `rb` | `Rigidbody2D` | Rigidbody2D của nhân vật |

#### Các phương thức

| Phương thức | Mô tả |
|---|---|
| `Awake()` | Khởi tạo singleton |
| `Update()` | Đọc `Input.GetAxisRaw` → lưu vào `movement` |
| `FixedUpdate()` | Gọi `rb.MovePosition()` với velocity |

---

## 🗂️ Dependency Map

```
FangAutoTilePacker ──────────────── FangAutoTile[]
         │                                │
         ▼                                ▼
FangAutoTilePackerEditor ──── FangAutoTileEditor
                                          │
                      TileCombinationSegment, TileDrawingItem
                      TemporaryTexture2DBuffer, TileSegment

PlayerMovement ──────── (cung cấp movement vector)
        │
        ▼  (hoặc dùng chung transform)
CharacterAppearance ──── Animator, SpriteRenderer, Resources
```

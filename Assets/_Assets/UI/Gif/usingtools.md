# 🛠️ Hướng Dẫn Sử Dụng Các Tool & Gợi Ý Cải Thiện

---

## PHẦN 1 — FangAutoTile Tool (Công cụ tạo Auto Tile)

### A. Tạo một FangAutoTile mới

#### Bước 1: Tạo asset tile
Trong **Project window**:
```
Chuột phải > Create > Fang > Tile
```
Đặt tên file (ví dụ: `GrassTile`).

#### Bước 2: Chuẩn bị texture đầu vào

Texture phải theo **định dạng Fang** (5 hàng × N cột):

```
width  = (Tile size) × (Số frame animation)
height = (Tile size) × 5
```

Ví dụ tile 16px, 3 frames → texture 48×80px.

| Hàng (từ dưới lên) | Ý nghĩa |
|---|---|
| Hàng 0 (dưới cùng) | Isolated (không có láng giềng) |
| Hàng 1 | Edge (chỉ 1 cạnh) |
| Hàng 2 | Corner bên ngoài |
| Hàng 3 | T-junction |
| Hàng 4 (trên cùng) | Fully connected |

> **Lưu ý:** Texture phải bật `Read/Write Enabled` trong **Texture Import Settings** nếu gặp lỗi.

#### Bước 3: Cấu hình Inspector

Chọn asset `GrassTile`, Inspector hiển thị 2 foldout:

**① Tile Settings**
- `Animation Min/Max Speed`: Tốc độ animation (đặt cả 2 bằng nhau nếu không muốn random)
- `Animation Start Time`: Thời điểm bắt đầu (offset các tile để không sync hoàn toàn)
- `Collider Type`: `None` (không va chạm), `Sprite` (theo hình sprite), `Grid` (theo ô)
- `Connectable Tiles`: Kéo thả các tile khác vào đây nếu muốn chúng "nối liền" với tile này

**② Tile Generation Settings**
- `Enable Padding`: ✅ Bật để tránh artifact (bleeding) ở góc tile
- `One Tile Per Unit`: ✅ Nếu tile = 1 unit
- `Pixels Per Unit`: Nhập thủ công nếu tắt `One Tile Per Unit`
- `Wrap Mode`: `Clamp` (thường dùng cho tile)
- `Filter Mode`: `Point` (pixel art)
- `Main Channel`: Kéo texture chính vào đây
- `Sub Channels`: Thêm texture phụ nếu cần (ví dụ: normal map, emission)

#### Bước 4: Generate

Nhấn nút **"Generate!"** (màu xanh lam, cao 50px).

Sau khi generate, Info Box hiển thị:
```
Generated:
  Combinations: 47
  Number of frames: 3
  Tile size: 16 x 16
  Texture size: 256 x 256
```

#### Bước 5: Đặt tile lên Tilemap
- Mở **Tile Palette** (`Window > 2D > Tile Palette`)
- Kéo asset `GrassTile` vào Palette
- Vẽ lên Tilemap — tile sẽ tự động chọn sprite phù hợp!

---

### B. Dùng FangAutoTile Packer (Atlas nhiều tile)

Packer nhóm nhiều tile vào **một texture atlas** để tối ưu hiệu năng.

#### Bước 1: Tạo Packer asset
```
Project > Chuột phải > Create > Fang > Packer
```

#### Bước 2: Cấu hình Packer
- `Targets`: Kéo tất cả `FangAutoTile` cần gộp vào đây
- `Enable Padding`, `Wrap Mode`, `Filter Mode`: Giống như tile riêng lẻ

> **Lưu ý:** Tất cả tile trong Packer **phải có cùng số Sub Channels**.

#### Bước 3: Generate
Nhấn **"Generate!"** trong Inspector của Packer. Tất cả tile sẽ được vẽ vào một atlas chung.

---

### C. Xóa dữ liệu đã generate

Nhấn **"Clear all generated contents"** (màu đỏ) trong Inspector tile.  
> ⚠️ Thao tác này xóa sprites/textures đã tạo, có thể gây mất reference trong Scene.

---

### D. OnMapPlayerController (Script mẫu grid movement)

Script này dùng cho **demo** — gắn vào GameObject nhân vật trong scene `Sample/sample.unity`.

**Cách tích hợp vào project:**

```csharp
// Thêm component vào Player Object
[RequireComponent(typeof(Rigidbody2D))]
public class OnMapPlayerController : MonoBehaviour { ... }
```

**Cấu hình:**
- `Speed Per Frame`: Tốc độ di chuyển giữa các ô (đơn vị: units/giây)
- Cần thêm `Rigidbody2D` (Kinematic) và collider vào Player

**Input mặc định:** Arrow Keys (←↑↓→)

---

## PHẦN 2 — Super_Retro_Collection Scripts

### A. PlayerMovement.cs — Di chuyển cơ bản

Gắn script vào nhân vật chính:

1. Thêm `Rigidbody2D` component (Body Type: Dynamic)
2. Gắn script `PlayerMovement`
3. Kéo `Rigidbody2D` vào field `rb`
4. Chỉnh `moveSpeed` (mặc định: 5)

**Input:** WASD hoặc Arrow Keys (qua `Input.GetAxisRaw`)

---

### B. CharacterAppearance.cs — Animation nhân vật

**Yêu cầu chuẩn bị:**

1. Đặt sprite sheet tại `Resources/Characters/{TênNhânVật}/spritesheet`  
   Ví dụ: `Resources/Characters/hero/spritesheet.png`
2. Sprite sheet phải được slice (Sprite Mode: Multiple)
3. Tên từng sprite trong sheet phải **khớp tên** trong Animator

**Gắn script:**
1. Thêm `Animator` và `SpriteRenderer` vào nhân vật
2. Gắn `CharacterAppearance`
3. Điền `SpriteSheetName` = tên thư mục sprite sheet (ví dụ: `hero`)
4. Kéo `Transform`, `Animator`, `SpriteRenderer` vào các field tương ứng

**Animator Parameters cần có:**

| Parameter | Kiểu | Điều kiện chuyển trạng thái |
|---|---|---|
| `speed` | Float | > 0.1 → Walk animation |
| `orientation` | Integer | 0=Up, 2=Left, 4=Down, 6=Right |

**Thay skin tại runtime:**
```csharp
// Đổi sprite sheet khi runtime
CharacterAppearance.instance.SpriteSheetName = "chara_02";
// Script tự load lại trong LateUpdate
```

---

## PHẦN 3 — Các Scene Mẫu

| Scene | Nội dung demo |
|---|---|
| `sample.unity` (FangAutoTile) | Demo auto-tile với OnMapPlayerController |
| `farm_sample.unity` | Môi trường nông trang |
| `forest_sample.unity` | Rừng cây |
| `gigantic_tree_sample.unity` | Cây cổ thụ lớn |
| `indoor_sample.unity` | Nội thất trong nhà |
| `marketplace_sample.unity` | Chợ |
| `prefabs_sample.unity` | Demo prefabs |
| `tower_sample.unity` | Tháp |
| `village_sample.unity` | Làng |
| `behavior_sample.unity` | Demo behavior script |

---

## PHẦN 4 — 🔍 Phân Tích & Gợi Ý Cải Thiện

---

### 🔴 Vấn đề nghiêm trọng

#### 1. `GetSuitableTextureSize()` — Lỗi tính `h` thay vì dùng `Height`

**Vị trí:** `FangAutoTileEditor.cs`, dòng 540-541 (và 598-599)

```csharp
// ❌ Lỗi: Dùng Width thay vì Height để tính h
int h = segmentOrdered.Current.Width;   // BUG!

// ✅ Đúng:
int h = segmentOrdered.Current.Height;
```

→ Với tile không vuông, texture atlas có thể bị overflow hoặc không đủ chỗ.

---

#### 2. `OnMapPlayerController` — Diagonal collision bug

**Vị trí:** `OnMapPlayerController.cs`, dòng 76-83

```csharp
// ❌ Lỗi: Cả 2 nhánh xét WalkableTestT đều set x = 0
if (Mathf.Abs(WalkableTestT.point.x - currentPos.x) > 0.25f) x = 0;
if (Mathf.Abs(WalkableTestT.point.y - currentPos.y) > 0.25f) x = 0; // ← nên là y = 0
```

→ Khi di chuyển chéo va chạm tường, `y` không bị chặn đúng.

---

#### 3. `CharacterAppearance` — Có thể crash `NullReferenceException`

**Vị trí:** `CharacterAppearance.cs`, dòng 76

```csharp
// ❌ Crash nếu sprite hiện tại không tồn tại trong dictionary
this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];

// ✅ An toàn hơn:
if (this.spriteSheet.TryGetValue(this.spriteRenderer.sprite.name, out var s))
    this.spriteRenderer.sprite = s;
```

---

### 🟡 Thiếu tính năng / Cần cải thiện

#### 4. `OnMapPlayerController` — Chưa hỗ trợ New Input System

```csharp
// ❌ Cũ: Input.GetKey (legacy)
x += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;

// ✅ Hiện đại: InputAction
[SerializeField] private InputAction moveAction;
var input = moveAction.ReadValue<Vector2>();
```

→ Cân nhắc port sang Unity Input System mới để dễ rebind phím và hỗ trợ gamepad.

---

#### 5. `PlayerMovement` — Singleton không thread-safe / không tự hủy

```csharp
// ❌ Nếu 2 scene load trùng nhau, singleton bị ghi đè
void Awake() { instance = this; }

// ✅ Chuẩn hơn:
void Awake()
{
    if (instance != null && instance != this) { Destroy(gameObject); return; }
    instance = this;
    DontDestroyOnLoad(gameObject); // nếu cần persist
}
```

---

#### 6. `CharacterAppearance` — Tính `movement` không chính xác ở frame đầu

```csharp
// FixedUpdate tính delta từ previousPosition
// Frame đầu tiên previousPosition = Start() → OK
// Nhưng khi teleport, delta sẽ rất lớn → animation glitch
```

→ Nên reset `previousPosition` khi teleport:
```csharp
public void Teleport(Vector2 newPos)
{
    transform.position = newPos;
    previousPosition = newPos; // Reset để tránh glitch
}
```

---

#### 7. `CharacterAppearance` — Orientation chỉ xét 4 hướng độc lập

```csharp
// ❌ Ưu tiên Y rồi X → có thể che lấp hướng khi di chuyển chéo
if (movement.x > 0) orientation = 6;
if (movement.x < 0) orientation = 2;
if (movement.y > 0) orientation = 0;  // Ghi đè horizontal!
if (movement.y < 0) orientation = 4;
```

→ Nên dùng `else if` hoặc xét magnitude:
```csharp
if (Mathf.Abs(movement.x) >= Mathf.Abs(movement.y))
{
    orientation = movement.x > 0 ? 6 : 2;
}
else
{
    orientation = movement.y > 0 ? 0 : 4;
}
```

---

#### 8. `FangAutoTileEditor` — Magic numbers không có comment

```csharp
// ❌ Không rõ 2341 từ đâu ra
int[] combinationIds = new int[2341];

// ✅ Nên thêm comment
// 12 bits (4 corners × 3 bits each) → max value = 2^12 = 4096... nhưng thực tế chỉ cần 5^4 = 625
// 2341 là giá trị encode tối đa của 4 quarter × 5 kind (0-4): (4 << 9) | (4 << 6) | (4 << 3) | 4 = 2340 + 1
const int MAX_COMBINATION_ID = 2341;
```

---

#### 9. FangAutoTile — Không hỗ trợ Diagonal Auto-tile riêng biệt

Hiện tại góc chéo chỉ được xét theo logic 4-bit quarter. Một số game cần xử lý **outer corner** riêng biệt (16-tile hoặc 48-tile). Cân nhắc thêm option chọn thuật toán tiling.

---

### 🟢 Gợi ý tính năng mới

| Tính năng | Mô tả | Độ khó |
|---|---|---|
| **Preview runtime trong Scene** | Hiển thị tile tại vị trí cursor khi đang paint | Trung bình |
| **Undo/Redo cho Generate** | Bọc Generate() trong `Undo.RecordObject()` | Dễ |
| **Export Atlas ra file PNG** | Cho phép dùng atlas bên ngoài Unity | Trung bình |
| **CharacterAppearance → unify với Animator Override** | Thay vì swap sprite, dùng Animator Override Controller | Cao |
| **PlayerMovement → Smooth Diagonal Speed** | Nhân hệ số `0.707f` khi di chuyển chéo | Dễ |
| **Configurable Input Keys** | Cho phép chỉnh phím trong Inspector | Dễ |

---

## Tóm Tắt Ưu Tiên Fix

| # | Vấn đề | File | Mức độ |
|---|---|---|---|
| 1 | Lỗi tính `h = Width` thay vì `Height` | `FangAutoTileEditor.cs` | 🔴 Critical |
| 2 | Diagonal collision set sai `x` thay vì `y` | `OnMapPlayerController.cs` | 🔴 Critical |
| 3 | Crash nếu sprite name không có trong dict | `CharacterAppearance.cs` | 🔴 Critical |
| 4 | Singleton không bảo vệ khỏi duplicate | `PlayerMovement.cs` | 🟡 Important |
| 5 | Orientation bị ghi đè khi di chuyển chéo | `CharacterAppearance.cs` | 🟡 Important |
| 6 | Teleport gây animation glitch | `CharacterAppearance.cs` | 🟡 Minor |
| 7 | Magic number `2341` không có comment | `FangAutoTileEditor.cs` | 🟢 Quality |

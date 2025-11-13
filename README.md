# 스파르타 던전 탐험

## 유니티 입문 개인과제
<img width="1000" height="600" alt="Image" src="https://github.com/user-attachments/assets/d93b2666-d967-4f47-9072-9ca767124d9c" />

## 1. 프로젝트 소개
유니티에서 3d이동과 물리처리를 학습하기 위한 프로젝트

---

### **개발 환경**
- **Engine**: Unity 2022.3.62f2
- **Language**: C# 
- **IDE**: Visual Studio

## 2. 프로젝트 구현 내용
 # 필수 과제

### 1. 기본 이동 및 점프
- Input System과 Rigidbody를 이용해 구현

### 2. 스태미나 바 UI 
- 체력을 사용하지 않아 체력바 대신 스태미나 바를 UI캔버스에 추가하여 스태미나 나타내도록 설정
  
### 3. 동적 환경 조사
- Raycast를 사용해 플레이어가 바라보는 오브젝트의 Layer를 검출해 오브젝트의 정보를 UI에 표시
- 게임 실행 메커니즘

### 4. 점프대
- 캐릭터가 밟을 때 위로 높이 튀어오르는 점프대 구현(ForceMode.Impulse사용)

### 5. 아이템 데이터
- 아이템 데이터를 ScriptableObject로 정의. 각 아이템의 이름, 설명, 속성 등을 ScriptableObject로 관리

### 6. 아이템 사용
- 특정 아이템 사용 후 효과가 일정 시간 동안 지속되는 시스템 구현
- 맵에 배치된 아이템이 플레이어가 닿아 사라진 후 일정 시간 후 리스폰 되게 구현


 # 도전 과제

### 1. 3인칭 시점
- 3인칭 카메라 시점을 설정하고 플레이어를 따라다니도록 설정

---
  
## 3. 사용 기술

### State Machine Pattern (FSM)
- 플레이어의 상태(Idle, Move, Run, Jump, Zoom)를 명확하게 분리하여 관리
- UpdateState() 메서드를 통해 현재 상태에 따른 로직을 switch문으로 처리
- 상태 전환이 명확하고 유지보수가 용이한 구조
  
<img width="592" height="443" alt="Image" src="https://github.com/user-attachments/assets/564d9e60-f3f7-4c1d-8355-ca6e661d2ad8" />

### ScriptableObject
- ItemData를 ScriptableObject로 구현하여 데이터와 로직 분리
- 아이템 타입(Buff, Equip, Potion)별로 데이터 관리
- Inspector에서 직접 수정 가능한 유연한 아이템 시스템 구축
- Use() 메서드를 통한 다형성 구현

<img width="492" height="433" alt="Image" src="https://github.com/user-attachments/assets/1ad62c8a-e5d1-4c19-a685-05799a5b1324" />

### Event Bus Pattern
- 정적 이벤트를 통한 느슨한 결합 구현
- 플레이어 행동 요청 이벤트 (점프, 달리기, 줌, 인벤토리)
- 플레이어 상태 변화 이벤트 (착지, 리스폰, 사망 등)
- 컴포넌트 간 직접 참조 없이 통신 가능한 구조

<img width="633" height="347" alt="Image" src="https://github.com/user-attachments/assets/782bcdb5-63d3-4b7d-885b-406289c95b7d" />


---

## 4. 인게임 스크린샷
<img width="1000" height="600" alt="Image" src="https://github.com/user-attachments/assets/bcc5244c-5889-439c-a375-bb6d5fcf8934" />
<img width="1000" height="600" alt="Image" src="https://github.com/user-attachments/assets/167a997a-9f73-4387-9358-493c930efb15" />
<img width="1000" height="600" alt="Image" src="https://github.com/user-attachments/assets/5f4f194d-4891-482d-abc2-e3adfb8a823a" />


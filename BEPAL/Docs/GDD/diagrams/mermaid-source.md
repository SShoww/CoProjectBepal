# Mermaid ต้นฉบับ (ก่อนแปลงเป็น Draw.io)

เก็บไว้เป็น Reference/สำรอง — ไดอะแกรมที่ใช้งานจริงคือไฟล์ .drawio ในโฟลเดอร์นี้ (ดึงจาก `git HEAD` ก่อนลบออกจาก GDD เมื่อ 2026-10-07)

## core-loop-day

มาจาก `01-core-loop.md` → [core-loop-day.drawio](core-loop-day.drawio)

```mermaid
flowchart TD
    Start([Start Day / วันใหม่]) --> Phase1[Phase 1: Narrative & Event Phase<br>รับเหตุการณ์สุ่มรายวัน / พ่อค้ามาเยือน / สัตว์บุก]
    
    Phase1 --> Phase2[Phase 2: Care & QTE Action Phase<br>ใช้ Energy ทำกิจกรรม: Feed / Clean / Train / Heal]
    
    Phase2 --> CheckEnergy{Energy หมด หรือ<br>เดินไปเตียง + Space สิ้นสุดวัน?}
    CheckEnergy -->|ยังไม่หมด| Phase2
    CheckEnergy -->|หมด หรือ นอนที่เตียง| Phase3[Phase 3: Defense & Resolution Phase<br>ต่อสู้รับมือภัยคุกคาม / ป้องกันฐาน]
    
    Phase3 --> CheckSurvive{สัตว์เลี้ยง<br>HP เหลือ 0 หรือไม่?}
    CheckSurvive -->|HP = 0| RevivePrompt[กู้ชีพฉุกเฉินคุณหมอ จ่าย 250G<br>ฟื้นฟูกลับมา HP = 1]
    RevivePrompt --> Phase4
    CheckSurvive -->|รอดชีวิต| Phase4[Phase 4: Progression & Save Phase<br>หักค่าสเตตัสรายวัน / สรุปผล / เซฟเกม]
    
    Phase4 -->|เข้าสู่วันถัดไป| Start
```

## core-loop-base

มาจาก `01-core-loop.md` → [core-loop-base.drawio](core-loop-base.drawio)

```mermaid
flowchart TD
    Base[ศูนย์กลางห้องพักพิง<br>Habitat Base] -->|คลิ๊กที่สัตว์| Care[เลือกหัวข้อการดูแล<br>Feed / Clean / Train / Heal]
    Care -->|ใช้ 1 AP| QTE[เล่น QTE วงล้อ 10 จังหวะ]
    QTE --> Reward[สะสม Progress bar ของสัตว์<br>และ EXP bar ของผู้เล่น]
    Reward --> Base
    Base -->|เปิดประตู Day 2+| Event{Event สุ่ม 3 แบบ}
    Event --> NewPet[เจอสัตว์ตัวใหม่]
    Event --> Merchant[เจอพ่อค้า]
    Event --> Disaster[เจอภัยธรรมชาติ]
    Base -->|สมุด| Book[pet discovery / Disaster]
    Base -->|Upgrade icon| Upg[Upgrade: QTE / Progress / Energy]
    Base -->|คลิ๊กหมอ| Doc[Doctor: ชุบชีวิตสัตว์ HP = 0]
    Base -->|End day| NewDay[วันใหม่: Stomach & Clean ลดลง]
    NewDay --> Base
```

## state-machine

มาจาก `03-mechanics.md` → [state-machine.drawio](state-machine.drawio)

```mermaid
stateDiagram-v2

    [*] --> DayStartChoice : เริ่มต้นวันใหม่

    DayStartChoice --> MorningEvent_Day1 : Day 1 (Safe Day / Tutorial)
    DayStartChoice --> MorningEvent_Day2 : Day 2 (Acid Puddle Clues)
    DayStartChoice --> MorningEvent_Day3 : Day 3 (Merchant Arrival)
    DayStartChoice --> MorningEvent_Endless : Day 4+ (Endless Random Events)

    MorningEvent_Day1 --> HabitatBaseRoom : เข้าสู่ห้องพักพิง
    MorningEvent_Day2 --> HabitatBaseRoom : เข้าสู่ห้องพักพิง
    MorningEvent_Day3 --> HabitatBaseRoom : เข้าสู่ห้องพักพิง
    MorningEvent_Endless --> HabitatBaseRoom : เข้าสู่ห้องพักพิง

    state HabitatBaseRoom {
        [*] --> IdleRoom : แสดงค่าสถานะ AP, Gold, Pet Stats
        IdleRoom --> CareQTE_Feed : กะจังหวะเล็งเป้าวงล้อเลือก Feed
        IdleRoom --> CareQTE_Clean : กะจังหวะเล็งเป้าวงล้อเลือก Clean
        IdleRoom --> CareQTE_Train : กะจังหวะเล็งเป้าวงล้อเลือก Train
        IdleRoom --> CareQTE_Heal : กะจังหวะเล็งเป้าวงล้อเลือก Heal
        IdleRoom --> UpgradeStationModal : คลิกสถานีอัปเกรดฐาน (ใช้ Skill Points/Gold)
        IdleRoom --> DoctorClinicModal : คลิกคลินิกคุณหมอ
        IdleRoom --> InventoryModal : คลิกไอคอนกระเป๋า (ใช้ไอเทม)
        IdleRoom --> SurvivalLogModal : คลิกสมุดบันทึก (Pet & Disaster Data)
        
        CareQTE_Feed --> IdleRoom : จบ 10 Attempts / หัก Energy / สะสม Progress
        CareQTE_Clean --> IdleRoom : จบ 10 Attempts / หัก Energy / สะสม Progress
        CareQTE_Train --> IdleRoom : จบ 10 Attempts / หัก Energy / สะสม Progress
        CareQTE_Heal --> IdleRoom : จบ 10 Attempts / หัก Energy / สะสม Progress
        UpgradeStationModal --> IdleRoom : ปิดหน้าต่างอัปเกรด
        DoctorClinicModal --> IdleRoom : ปิดคลินิก
        InventoryModal --> IdleRoom : ปิดกระเป๋า
        SurvivalLogModal --> IdleRoom : ปิดสมุด
    }

    HabitatBaseRoom --> Phase3_Defense : Energy == 0 หรือ กด 'E' สิ้นสุดวัน

    state Phase3_Defense {
        [*] --> RouteDayEvent
        RouteDayEvent --> Day1_Safe : Day 1 (ไม่มีภัยคุกคาม)
        RouteDayEvent --> Day2_Encounter : Day 2 (Toothless Knock Knock)
        RouteDayEvent --> Day3_Merchant : Day 3 (Merchant Buyout Dilemma)
        RouteDayEvent --> Endless_Encounters : Day 4+ (สุ่มเจอศัตรู หรือ ภัยธรรมชาติ)
        
        Day1_Safe --> Phase4_Progression
        
        Day2_Encounter --> Chase_Resolution : เลือก [CHASE] ขับไล่
        Day2_Encounter --> Taming_Combat : เลือก [TAME] ฝึกให้เชื่อง (Dodge/Counter)
        Taming_Combat --> ToothlessUnlocked : ชนะการต่อสู้
        Chase_Resolution --> Phase4_Progression
        ToothlessUnlocked --> Phase4_Progression
        
        Day3_Merchant --> Buyout_Choice : พ่อค้าเสนอซื้อสัตว์ 5,000G
        Buyout_Choice --> SoldPet : ตอบ [YES] ยอมขาย
        SoldPet --> GameOver_Check : ตรวจสอบว่าเหลือสัตว์เลี้ยงหรือไม่?
        GameOver_Check --> Game_Over : สัตว์เลี้ยงหมดฐาน (Fail State)
        GameOver_Check --> Phase4_Progression : ยังมีสัตว์ตัวอื่นเหลืออยู่
        
        Buyout_Choice --> BossCombat_Arena : ตอบ [NO] ปฏิเสธ (เข้าสู่ Combat QTE)
        BossCombat_Arena --> BossVictory : โจมตีสวนกลับสำเร็จครบ 5 ครั้ง
        BossVictory --> Phase4_Progression
        
        Endless_Encounters --> RandomCombat_QTE : มินิเกมต่อสู้ศัตรูสุ่ม (Dodge/Attack)
        Endless_Encounters --> RandomDisaster_QTE : มินิเกมแก้ปัญหาภัยธรรมชาติสุ่ม
        RandomCombat_QTE --> Phase4_Progression : สำเร็จ
        RandomDisaster_QTE --> Phase4_Progression : สำเร็จ
    }

    Phase3_Defense --> EmergencyRevive : สัตว์เลี้ยง HP == 0 
    EmergencyRevive --> Phase4_Progression : จ่าย 250G ชุบชีวิตให้ HP = 1
    EmergencyRevive --> Game_Over : เงินไม่พอ 250G และสัตว์เลี้ยงตายหมด

    state Phase4_Progression {
        [*] --> ApplyDailyDecay : คำนวณหักสเตตัสรายวัน (Stomach, Clean)
        ApplyDailyDecay --> SicknessCheck : ตรวจสอบว่าสเตตัสวิกฤตจนป่วยและ HP ลดหรือไม่
        SicknessCheck --> CheckLevelUp : ประมวลผล Progress สัตว์และผู้เล่น
        CheckLevelUp --> DailySummaryReportCard : แสดงเกรด, Gold, Log และเซฟเกม
        DailySummaryReportCard --> NightRestTransition : พักผ่อนข้ามคืน
    }

    NightRestTransition --> DayStartChoice : ก้าวสู่วันใหม่ (ฟื้นฟู Energy / สุ่ม Event ต่อไป)
    Game_Over --> [*]
```

## scene-flow

มาจาก `04-class-diagram.md` → [scene-flow.drawio](scene-flow.drawio)

```mermaid
flowchart LR
    MainMenu --> ChooseStarter --> Base
    Base -->|Space ที่สัตว์| CareSelect --> Qte --> Base
    Base -->|เตียง| Night --> Base
    Base --> Upgrade & Doctor & Notebook
    Base -->|Esc| Paused[Pause: Resume / Main Menu]
    Base -->|ประตู| DayEvents
    DayEvents -->|Day 2| Fight
    DayEvents -->|Day 3| Shop
    DayEvents -->|Day 4| Storm[Storm: Clean -50 ทุกตัว + ฝน]
    DayEvents -->|Day 5| Fight --> ToBeContinued
    Fight -->|แพ้ Toothless| GameOver --> ChooseStarter
```

## narrative-days

มาจาก `11-narrative-world.md` → [narrative-days.drawio](narrative-days.drawio)

```mermaid
flowchart TD
    D1[Day 1: The Arrival] -->|เลือก Starter Pet| D2[Day 2: The Wild Infiltration]
    D2 -->|เสียงเคาะประตู 'Knock Knock' + รับมือ Toothless| D2_Choice{ทางเลือก: ขับไล่ หรือ เชื่อง?}
    D2_Choice -->|Chase| D2_End[Toothless หนีไป / จบวันอย่างสงบ]
    D2_Choice -->|Tame| D2_Combat[เข้าสู่ฉาก Combat Taming / สยบ Toothless เข้าทีม]
    D2_Combat --> D3[Day 3: The Traveling Merchant]
    D2_End --> D3
    D3 -->|พ่อค้าเร่เดินทางมาถึงพร้อมข้อเสนอซื้อสัตว์เลี้ยง| D3_Choice{ยอมขาย Toothless 5,000G?}
    D3_Choice -->|Sell| D3_EndingA[จบแบบ Bittersweet: ได้เงินมหาศาลแต่สูญเสียสัตว์เลี้ยง]
    D3_Choice -->|Refuse| D3_Boss[เข้าสู่ Boss Fight: Merchant Battle ปกป้องบ้าน]
    D3_Boss -->|Victory| D3_EndingB[จบแบบ Heroic: ปกป้องสัตว์เลี้ยงสำเร็จและขับไล่พ่อค้า]
    D3_EndingB --> D_Final[Final Day: Disaster]
    D_Final -->|Thunderstorm| D_EndSlice[ดูแล ทำความสะอาดสัตว์เลี้ยง]
```

## scene-breakdown

มาจาก `13-scene-breakdown.md` → [scene-breakdown.drawio](scene-breakdown.drawio)

```mermaid
flowchart LR
    S1[S1 Main Menu] -->|Play| S2[S2 Choose first pet]
    S1 -->|Option| OPT[Option]
    S2 --> S3[S3 Habitat Base]
    S3 --> S4[S4 Care pet]
    S4 --> S5[S5 QTE Train] & S6[S6 QTE Feed] & S7[S7 QTE Clean] & S8[S8 QTE Heal]
    S5 & S6 & S7 & S8 -->|ครบ 10 ครั้ง| S3
    S3 -->|สมุด| S9[S9 สมุดหลัก]
    S9 --> S10[S10 pet discovery] & S11[S11 Disaster]
    S3 -->|Upgrade icon| S12[S12 Upgrade]
    S3 -->|หมอ| S13[S13 Doctor]
    S3 -->|End day → วันใหม่| S14[S14 Event: Knock Knock !!]
    S14 --> S15[S15 pet encounter] & S20[S20 Merchant] & DIS[Disaster event]
    S15 --> S16[S16 pet choice]
    S16 -->|Chase| S3
    S16 -->|Tame| S17[S17 choose pet to fight]
    S17 --> S18[S18 fight]
    S18 -->|ศัตรู HP = 0| S19[S19 You got new pet !!!]
    S18 -->|สัตว์เราตาย| S17
    S18 -->|ตายหมด: Game Over| S2
    S19 --> S3
    S20 --> S3
```

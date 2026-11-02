### 2026-10-13 — Q-20261013-validate-ps1 — แก้ validate.ps1 (C10) ให้รับ path + รู้จักไอเทม/ราคา Point ใหม่
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: `validate.ps1` ไม่ฝัง path, ไม่ crash กับคอลัมน์ `starter`, ราคา upgrade ตรง Balance ใหม่, ไม่ผิดกับร้านหลัง Sell, และตรวจ item events
- Files: BEPAL/Docs/Balance/tools/validate.ps1, BEPAL/Docs/Balance/telemetry-spec.md, Context/Status.md
- Context: balance-report C10; พารามิเตอร์ `-Raw/-Csv/-Out` (alias ของ `$RawArg/$CsvArg/$OutArg` เพราะ `$csv`/`$out` ถูกใช้เป็นตัวแปรในสคริปต์ และ PowerShell ไม่สนตัวพิมพ์); I09 เลิกห้าม shop_buy หลัง Sell; ใหม่ I16 (shop_buy ราคา/วัน/เพดาน 3, item_use มีของ/`left`/target, tonic == energy(tonic), shielded hit dmg 0 และ <= 2 x Glasses); วันที่ใช้ InvariantCulture (เดิมออกปี 2569)
- Validation (2026-10-13): batch 50 run (10 seeds x 5 profile) หลังข้อ 6 -> 16/16 invariant PASS; ทดสอบด้านลบด้วยสำเนาที่ถูกแก้ (ราคา 999, `left`=7) -> I01 และ I16 จับได้. ไม่ได้ลองกับ data คนจริง (ไฟล์ดิบ data-human-raw ไม่อยู่ในเครื่อง/ไม่อยู่ใน git) จึงยังไม่ยืนยันว่าข้อมูลเก่า (ราคา Energy 3 point ก่อนข้อ 5) จะ FAIL I06 ตามคาดหรือไม่; ค่าเริ่มต้น `data/raw` ยังไม่มีใน repo
- Next action: ผู้ใช้ยืนยันปิด; เมื่อเก็บ telemetry คนจริงรอบใหม่ให้รัน validate ด้วย `-Raw/-Csv/-Out`

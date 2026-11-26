# CSVS GOD MODE — TAMIL & ENGLISH BILINGUAL SIGNAGE REPORT
**Version:** V0.2.1  
**Studio:** CSVS IT Tech  
**Setting:** Karaikal, Puducherry, India  
**Date:** November 26, 2026  

---

## 1. Bilingual Signage System

To capture the cultural identity of Puducherry and Karaikal, `ModularIndianBuildingSystem.cs` and `HUDController.cs` implement a bilingual display system featuring authentic Tamil (தமிழ்) and English typography:

### 1.1 Commercial Signboards
All commercial bazaars generated along Kamarajar Salai and Bharathiyar Road feature dual-language storefront canopies:
- **Silk & Textiles:** காரைக்கால் பட்டு மையம் | KARAIKAL SILK PALACE
- **Jewellers:** ஸ்ரீ மீனாட்சி ஜூவல்லரி | SRI MEENAKSHI JEWELLERS
- **Medical / Pharmacy:** சாந்தி மெடிக்கல்ஸ் | SHANTHI MEDICALS
- **Departmental Store:** பாரதி சூப்பர் மார்க்கெட் | BHARATHI SUPER MARKET
- **Bakery & Sweets:** அன்னை பேக்கரி & ஸ்வீட்ஸ் | ANNAI BAKERY & SWEETS
- **South Indian Eatery:** கணேஷ் பவன் ஹோட்டல் | HOTEL GANESH BHAVAN
- **Tailoring & Fashion:** பாபு டெய்லர்ஸ் | BABU TAILORS
- **Hardware & Electricals:** ராஜா ஹார்டுவேர்ஸ் | RAJA HARDWARES

### 1.2 District Banners & GPS HUD
The GTA-inspired HUD dynamically displays bilingual district titles as the player flies or walks between sectors:
- **Town Center:** காரைக்கால் நகரம் | KARAIKAL TOWN CENTER
- **Beach Promenade:** காரைக்கால் கடற்கரை | KARAIKAL BEACH
- **Arasalar Riverfront:** அரசலாறு ஆறு | ARASALAR RIVERFRONT
- **Heritage Zone:** பாரம்பரிய பகுதி | HERITAGE & TEMPLE ZONE
- **Transport Hub:** போக்குவரத்து மையம் | TRANSPORT HUB
- **Port Industrial:** காரைக்கால் துறைமுகம் | KARAIKAL PORT
- **Rural Outskirts:** திருநள்ளாறு வழி | THIRUNALLAR OUTSKIRTS
- **Modern Expansion:** நவீன வளாகம் | MODERN HIGH-RISE DISTRICT

### 1.3 Font Rendering Pipeline
- Uses Unity TextMeshPro with dynamic UTF-8 Unicode font fallback supporting Tamil block range `U+0B80` to `U+0BFF`.
- High-contrast color styling: Pure White (`#FFFFFF`) with deep drop-shadow (`#00000088`) on vibrant yellow, cobalt blue, or emerald green signboard backdrops.

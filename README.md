# Tower Defense Case Project

Oyun Motoru: Unity
Versiyon: Unity 6 (6000.0.32f1)

Ana Menü
Ana menüde oyundaki üç bölümün içeriğini gösteren paneller bulunmaktadır. 

Oyun Döngüsü
Düşmanlar dalga şeklinde gelerek oyuncunun savunması gereken kaleye belirli bir rotayı izleyerek ilerlerler. Eğer kalenin canı sıfırın altına inerse “Game Over” paneli ortaya çıkar ve bölüm kaybedilmiş olur. Oyuncunun bölümü başarılı bir şekilde bitirmesi için tüm dalgalardaki düşmanları öldürmesi gerekir. Tüm düşmanlar öldüğünde “Level Complete” paneli ortaya çıkar ve bölüm başarıyla tamamlanmış olur. 

Oyuncunun kullandığı karakterin canı sıfırın altına indiği zaman ekranın sağ alt köşesinde bir “Respawn” butonu belirir. Bu butonun aktif olmasının belirli bir süresi vardır. Bu süre geçtikten sonra oyuncu o butona basarak karakterini tekrar diriltebilir.

Karakterler
Oyunda iki tane oynanabilir karakter bulunmaktadır. Oyuncu bölüm başlarında ekranda gözüken panelden iki karakter arasındaki farkları görebilir ve seçimini yapabilir.

Düşmanlar
Slime: Hızlı ve canı az olan bir düşman türü. Oyuncuya hasar veremez ve oyuncuyu görünce durmaz. Sadece kaleye odaklanır ve kaleye hasar verir.
Orc: Orta hızda ve orta seviyede cana sahiptir. Oyuncu saldırı alanına girerse durur ve oyuncuya zarar vermeye başlar. Eğer oyuncu saldırı alanından çıkarsa Orc kaleye gitmeye devam eder. Aynı anda hem kaleye hem de oyuncuya hasar veremez, sadece bir tanesine hasar verebilir
Knight: Yavaş ama ağır hasarı olan bir düşman. Orc ile aynı mantıkta çalışır.

Boss: Üçüncü bölümün son dalgasında ortaya çıkar. Belirli süre aralıklarıyla “Buff” veya “Protection” adlı yeteneklerden birisini gerçekleştirir. Aynı anda hem oyuncuya hem de kaleye hasar verebilir.
Buff: Boss’un etrafındaki belirli bir alanın içinde bulunan tüm düşmanların hasarını bir süreliğine arttırır. Buff etkisinde olan düşmanların can barlarının altında kırmızı bir ok sembolü gözükür.
Protection: Boss’un etrafındaki belirli bir alanın içinde bulunan tüm düşmanların aldığı hasarı bir süreliğine azaltır. Protection etkisinde olan düşmanların can barlarının altında mavi bir ok sembolü gözükür.

Oyun Ekranı

Next Wave Button: Ekranın sağ üstünde bulunan, bir sonraki dalgayı erkenden çağırmaya yarayan buton.
Restart Button: Ekranın sol üstünde bulunan, bölümü tekrar açmaya yarayan buton.
Main Menu Button: Restart butonunun sağında bulunan, ana menüye dönme butonu.
Player Health Bar: Ekranın sol altında bulunan, karakterin o anki canını gösteren can barı.
Respawn Button: Karakter öldüğü zaman ekranın sağ altında beliren, karakteri tekrar diriltmeye yarayan buton.
Wave Text: Ekranın üst kısmında bulunan, oyuncunun bulunduğu dalga ve bölümdeki toplam dalga sayısını gösteren yazı.

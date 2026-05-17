# システム全体の概要
hand_sender側に記載
# SerializeFieldについて
いくつかの変数にSerialzieField属性を付与することでリアルタイムで値を変更できるようにし  
モーターの調整を容易に行えるようにしました。
# SerializeField属性の変数
| 変数名 | 内容 |
| ---- | ---- |
| send_IP | 送信先の無線通信用マイコンのIPv4アドレス |
| 〇〇_coe | 対応する各モータの倍率 |
| state | 送信データを表示するためのTMPのtext |

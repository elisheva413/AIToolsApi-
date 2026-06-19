use("StoreMongoHW");

db.categories.deleteMany({});
db.products.deleteMany({});
db.users.deleteMany({});
db.orders.deleteMany({});

db.categories.insertMany([
  { _id: 105, name: "Braclets" },
  { _id: 110, name: "Charms" },
  { _id: 120, name: "Earrings" },
  { _id: 145, name: "Gift Card" },
  { _id: 115, name: "Necklaces" },
  { _id: 100, name: "Rings" },
  { _id: 125, name: "בלעדי לאתר" },
  { _id: 135, name: "חדש" },
  { _id: 130, name: "תכשיטי חריטה" }
]);

db.products.insertMany([
  {
    _id: 1,
    categoryId: 100,
    name: "טבעת",
    price: 190,
    quantity: 1,
    isActive: true
  },
  {
    _id: 2,
    categoryId: 100,
    name: "טבעת כסף חותם לב ניתנת לחריטה",
    price: 300,
    quantity: 11,
    isActive: true
  },
  {
    _id: 3,
    categoryId: 100,
    name: "טבעת כסף כפולה פפיון נוצץ",
    price: 380,
    quantity: 18,
    isActive: true
  },
  {
    _id: 4,
    categoryId: 100,
    name: "טבעת כסף איטרניטי נוצצת",
    price: 380,
    quantity: 7,
    isActive: true
  },
  {
    _id: 5,
    categoryId: 100,
    name: "טבעת כסף גל זרקונים",
    price: 189,
    quantity: 8,
    isActive: true
  }
]);

db.users.insertMany([
  {
    _id: 1,
    userName: "eli7p@gmail.com",
    firstName: "Elisheva",
    lastName: "Polsky",
    role: "User"
  },
  {
    _id: 2,
    userName: "zehavaleh@gmail.com",
    firstName: "זהבה גולדה",
    lastName: "קורנווסר",
    phone: "0527199547",
    address: "מנחם מנדל",
    role: "User"
  },
  {
    _id: 3,
    userName: "admin@pan.com",
    firstName: "Admin",
    lastName: "Pandora",
    phone: "0505555555",
    address: "Tel Aviv",
    role: "Admin"
  },
  {
    _id: 4,
    userName: "EL7P@.com",
    firstName: "פולסקי",
    lastName: "אלישבע",
    phone: "0548401866",
    address: "רחוב אבן דנן",
    role: "User"
  },
  {
    _id: 5,
    userName: "ruvka@example.com",
    firstName: "מילר",
    lastName: "רבקה",
    phone: "0509786800",
    address: "רחוב יפעוזיא 23",
    role: "User"
  }
]);

db.orders.insertMany([
  {
    _id: 5225,
    userId: 1,
    orderDate: "2026-03-16",
    orderSum: 630,
    orderStatus: "Delivered",
    items: [
      { productId: 2, quantity: 1 },
      { productId: 3, quantity: 1 }
    ]
  },
  {
    _id: 5230,
    userId: 1,
    orderDate: "2026-03-16",
    orderSum: 2460,
    orderStatus: "Delivered",
    items: [
      { productId: 3, quantity: 3 },
      { productId: 4, quantity: 2 }
    ]
  },
  {
    _id: 5235,
    userId: 1,
    orderDate: "2026-03-16",
    orderSum: 500,
    orderStatus: "Paid",
    items: [
      { productId: 5, quantity: 2 }
    ]
  },
  {
    _id: 5245,
    userId: 1,
    orderDate: "2026-03-16",
    orderSum: 1440,
    orderStatus: "Delivered",
    items: [
      { productId: 2, quantity: 2 },
      { productId: 4, quantity: 2 }
    ]
  },
  {
    _id: 5250,
    userId: 2,
    orderDate: "2026-03-16",
    orderSum: 480,
    orderStatus: "Delivered",
    items: [
      { productId: 1, quantity: 2 }
    ]
  }
]);
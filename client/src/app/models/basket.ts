export interface BasketItem {
  productId: number;
  name: string;
  price: number;
  prictureURL: string;
  brand: string;
  type: string;
  quantity: number;
}

export interface Basket {
  id: number;
  buyerId: string;
  items: BasketItem[];
}

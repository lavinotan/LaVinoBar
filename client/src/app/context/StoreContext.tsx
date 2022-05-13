import { createContext, PropsWithChildren, useContext, useState } from "react";
import { Basket } from "../models/basket";

interface StoreContextValue {
  basket: Basket | null;
  setBasket: (basket: Basket) => void;
  removeItem: (productId: number, quantity: number) => void;
}

export const StoreContext = createContext<StoreContextValue | undefined>(
  undefined
);

export function useStoreContext() {
  const context = useContext(StoreContext);

  if (context == undefined) {
    throw Error("Oops - we dot not seem to be in the provider");
  }

  return context;
}

// anything used inside of StoreProvider will be children
export function StoreProvider({ children }: PropsWithChildren<any>) {
  const [basket, setBasket] = useState<Basket | null>(null);

  function removeItem(productId: number, quantity: number) {
    if (!basket) return;

    const items = [...basket.items]; // to copy items first
    const itemIndex = items.findIndex((i) => i.productId == productId);

    if (itemIndex >= 0) {
      items[itemIndex].quantity -= quantity;
      if (items[itemIndex].quantity == 0) items.splice(itemIndex, 1); // remove item if quantity is 0
      setBasket((prevState) => {
        return { ...prevState!, items };
      });
    }
  }

  return (
    <StoreContext.Provider value={{ basket, setBasket, removeItem }}>
      {children}
    </StoreContext.Provider>
  );
}

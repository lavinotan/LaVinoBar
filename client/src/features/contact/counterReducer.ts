export const INCREMENT_COUNTER = "INCREMENT_COUNTER"; // action types
export const DECREMENT_COUNTER = "DECREMENT_COUNTER";

export interface CounterState {
  data: number;
  title: string;
}

const initialState: CounterState = {
  data: 42,
  title: "Reduct Counter",
};

// action creators
export function increment(amount = 1) {
  return {
    type: INCREMENT_COUNTER,
    payload: amount,
  };
}

export function decrement(amount = 1) {
  return {
    type: DECREMENT_COUNTER,
    payload: amount,
  };
}

export default function counterReducer(state = initialState, action: any) {
  switch (action.type) {
    case INCREMENT_COUNTER:
      // to avoid mutating state
      return {
        ...state,
        data: state.data + action.payload,
      }; // return as object

    case DECREMENT_COUNTER:
      // to avoid mutating state
      return {
        ...state,
        data: state.data - action.payload,
      };

    default:
      return state; // no matter what, return state
  }
}

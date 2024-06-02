import { Error } from "./error"

export type Result<T> = {
    value?:T,
    error?:Error,
    isSuccess:boolean,
    isFailure:boolean
}
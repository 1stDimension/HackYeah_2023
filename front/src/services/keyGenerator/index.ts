import axios from "axios";
import { postKeyParamsType } from "./models";

export const postKeyParams = async ({
  alghoritmType,
  keyLength,
  keyName,
}: postKeyParamsType) => {
  const body = {
    key_name: keyName,
    key_type: alghoritmType,
    key_size: keyLength,
  };
  return await axios.post("url", body);
};

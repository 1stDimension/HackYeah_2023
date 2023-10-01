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
  if (process.env.NEXT_PUBLIC_V1)
    return await axios.post(process.env.NEXT_PUBLIC_V1 + "/generate_key", body);
};

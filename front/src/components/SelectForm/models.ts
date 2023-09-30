export interface ISelectFormProps {
  inputLabel: string;
  name: string;
  options: string[] | number[];
  handleChange: (event: any) => void;
  value: string | number;
  helperLabel?: string;
}

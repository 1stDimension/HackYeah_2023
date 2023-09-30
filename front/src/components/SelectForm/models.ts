export interface ISelectFormProps {
  name: string;
  options: string[] | number[];
  handleChange: (event: any) => void;
  value: string | number;
  helperLabel?: string;
}

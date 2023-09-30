export interface ITextFieldFormProps {
  name: string;
  inputLabel: string;
  handleInputChange: (event: any) => void;
  value: string;
  helperLabel?: string;
}

            set
            {
                codeBox.Document.Blocks.Clear();
                var paragraph = new Paragraph(new Run(value));
                codeBox.Document.Blocks.Add(paragraph);
            }

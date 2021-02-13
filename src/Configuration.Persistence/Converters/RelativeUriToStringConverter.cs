namespace Kritikos.Configuration.Persistence.Converters
{
	using System;

	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class RelativeUriToStringConverter : ValueConverter<Uri, string>
	{
		private static readonly Uri Fallback = new("about:blank");

		public RelativeUriToStringConverter(
			Uri baseUri,
			ConverterMappingHints? mappingHints = null)
			: base(
				v => baseUri.MakeRelativeUri(v).ToString(),
				v => FromString(baseUri, v),
				mappingHints)
		{
		}

		private static Uri FromString(Uri baseUri, string relative)
			=> Uri.TryCreate(baseUri, relative, out var uri)
				? uri
				: Fallback;
	}
}

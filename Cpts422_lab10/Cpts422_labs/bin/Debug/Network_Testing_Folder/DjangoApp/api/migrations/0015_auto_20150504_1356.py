# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0014_delete_changesel'),
    ]

    operations = [
        migrations.AlterField(
            model_name='api',
            name='api_text',
            field=models.CharField(max_length=200, editable=False),
            preserve_default=True,
        ),
    ]
